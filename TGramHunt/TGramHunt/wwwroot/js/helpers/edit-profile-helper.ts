import * as $ from "jquery";
import * as Croppie from "croppie";
import { LinkHelper } from "./link-helper";

class EditProfileHelper {
    private imgHolderCallback: () => void | null;
    private imgHolder: HTMLImageElement | null;
    private croppie: Croppie | null;

    readonly opt = {
        win_ratio: 1,
        final_size: { w: 150, h: 150 },
        container: { width: 200, height: 200 },
        viewport: { width: 100, height: 100 },
    };

    read_file_from_input(input: HTMLInputElement, callback: () => void) {
        const self = this;
        if (input.files && input.files[0]) {
            this.imgHolderCallback = callback;
            const reader = new FileReader();
            if (!this.imgHolder) {
                this.imgHolder = new Image();
                this.imgHolder.onload = function () {
                    if (self.imgHolderCallback) {
                        self.imgHolderCallback();
                    }
                };
                this.imgHolder.onerror = function () {
                    self.show_alert("File is invalid.");
                };
            }

            reader.onload = function (e) {
                if (self.imgHolder) {
                    self.imgHolder.src = <string>e.target.result;
                }
            };

            reader.readAsDataURL(input.files[0]);
        } else {
            console.warn("failed to read file");
        }
    }

    show_alert(txt: string) {
        const modalHolder = $("#modal");
        modalHolder
            .find("#modalBody")
            .html($("#alert-dialog").html())
            .find(".alert-dialog-class")
            .text(txt);
        modalHolder.modal("show");
    }

    edit_profile_save_form() {
        const formData = new FormData();
        formData.append(
            "Name",
            <string | null>$("#edit-profile-form #name").val()
        );
        const linkHelper = new LinkHelper();

        $.ajax({
            type: "POST",
            data: formData,
            url: "/editprofile/index?handler=save",
            contentType: false,
            processData: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader(
                    "XSRF-TOKEN",
                    <string | null>(
                        $(
                            '#edit-profile-form input:hidden[name="__RequestVerificationToken"]'
                        ).val()
                    )
                );
            },
            success: function () {
                document.location.href = <string>(
                    $("#profile-link-redirect").val()
                );
            },
            error: function (err: JQuery.jqXHR) {
                if (err.status === 401) {
                    document.location.href = linkHelper.mainPage();
                } else if (err.status === 404) {
                    document.location.href = linkHelper.notFoundPage();
                }

                const $editProfForm = $("#edit-profile-form");
                $editProfForm.find(".error").html("").hide();

                if (err.responseJSON && err.responseJSON["Name"]) {
                    $editProfForm
                        .find(".error[for=name]")
                        .html(err.responseJSON["Name"].join(". "))
                        .show();
                } else if (err.responseText) {
                    $editProfForm
                        .find(".error[for=total]")
                        .html(err.responseText)
                        .show();
                } else {
                    $editProfForm
                        .find(".error[for=total]")
                        .html(err.toString())
                        .show();
                }
            },
        });
    }

    update_options_for_width(w: number) {
        const o = this.opt,
            vp_ratio = o.final_size.w / o.final_size.h,
            h = Math.floor(w / o.win_ratio);
        let new_vp_w, new_vp_h;
        w = Math.floor(w * 0.9);
        o.container.width = w;
        o.container.height = h;
        new_vp_h = 0.5 * h;
        new_vp_w = new_vp_h * vp_ratio;
        if (new_vp_w > 0.5 * w) {
            new_vp_w = 0.5 * w;
            new_vp_h = new_vp_w / vp_ratio;
        }
        new_vp_w = Math.floor(new_vp_w);
        new_vp_h = Math.floor(new_vp_h);
        o.viewport.height = new_vp_h;
        o.viewport.width = new_vp_w;
    }

    show_modal() {
        if ($("#imgModal-msg").width() > 0) {
            this.update_options_for_width($("#imgModal-msg").width());
            $("#imgModal-btnSave").hide();
            $("#imgModal-btnCrop").show();
            $("#cropped-img-wrp").html('<img id="cropped-img" />');
            this.show_croppie(<HTMLImageElement>$("#imgModal-cropme")[0]);
        }
    }

    show_croppie(img: HTMLImageElement) {
        const self = this;

        $(img).removeClass("ready");
        self.croppie?.destroy();

        const container = self.opt.container;
        const viewport = self.opt.viewport;

        self.croppie = new Croppie($(img)[0], {
            viewport: {
                width: viewport.width,
                height: viewport.height,
                type: "circle",
            },
            boundary: { width: container.width, height: container.height },
            showZoomer: true,
            mouseWheelZoom: true,
            enableZoom: true,
        });

        self.croppie.bind({
            url: self.imgHolder.src,
        });
    }

    crop_into_img(img: HTMLImageElement, callback: () => void) {
        const self = this;
        const final_size = self.opt.final_size;
        this.croppie
            .result({
                type: "base64",
                format: "png",
                circle: true,
                size: { width: final_size.w, height: final_size.h },
            })
            .then(function (resp) {
                $(img).attr("src", resp);
                $(img).removeClass("ready");

                self.croppie.destroy();
                self.croppie = null;

                if (callback) {
                    callback();
                }
            });
    }

    upload_image(img: HTMLImageElement) {
        const imgCanvas = document.createElement("canvas"),
            imgContext = imgCanvas.getContext("2d");
        imgCanvas.width = img.width;
        imgCanvas.height = img.height;
        imgContext.drawImage(img, 0, 0, img.width, img.height);
        const dataURL = imgCanvas.toDataURL();
        const linkHelper = new LinkHelper();

        $.ajax("/editprofile/index?handler=imagesave", {
            type: "POST",
            data: {
                imgBase64: dataURL,
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader(
                    "XSRF-TOKEN",
                    <string>(
                        $(
                            '#profile input:hidden[name="__RequestVerificationToken"]'
                        ).val()
                    )
                );
            },
            success: function (response) {
                const $profile_avatar = $(".profile_avatar");
                $profile_avatar.find("i").hide();
                $profile_avatar
                    .find("img")
                    .attr("src", response.link)
                    .removeClass("d-none");
                $("#croppmodal").modal("hide");
            },
            error: function (err) {
                if (err.status === 401) {
                    document.location.href = linkHelper.mainPage();
                } else if (err.status === 404) {
                    document.location.href = linkHelper.notFoundPage();
                }
            },
        });
    }
}

export { EditProfileHelper };
