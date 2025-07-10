import * as $ from "jquery";
import "jquery-validation";
import "jquery-validation/dist/additional-methods.js";
import "croppie";
import { EditProfileHelper } from "./helpers/edit-profile-helper";
import { LinkHelper } from "./helpers/link-helper";

jQuery.validator.addMethod("editProfileNameOneWordLength", function (value) {
    if (!value || value.indexOf(" ") !== -1) {
        return true;
    }

    const valLength = value.trim().length;
    return valLength >= 3 && valLength <= 255;
});

jQuery.validator.addMethod(
    "editProfileNameTwoWordLengthFirstPart",
    function (value) {
        if (!value) {
            return true;
        }
        value = value.trim();
        const index = value.indexOf(" ");
        if (index === -1) {
            return true;
        }
        const firstPart = value.substring(0, index).trim();
        const firstPartLength = firstPart.length;

        return firstPartLength >= 3 && firstPartLength <= 123;
    }
);

jQuery.validator.addMethod(
    "editProfileNameTwoWordLengthSecondPart",
    function (value) {
        if (!value) {
            return true;
        }
        value = value.trim();
        const index = value.indexOf(" ");
        if (index === -1) {
            return true;
        }
        const secondPart = value.substring(index);
        const secondPartLength = secondPart.trim().length;

        return secondPartLength >= 3 && secondPartLength <= 123;
    }
);

$(function () {
    const helper = new EditProfileHelper();
    $("#edit-profile-form #name").on("keyup", function (e) {
        const inputElem = <HTMLInputElement>e.target;
        let val = inputElem.value.trim();
        if (!val) {
            return;
        }

        val = val.trim();
        const indx = val.indexOf(" ");
        if (indx === -1) {
            return;
        }

        const secondPart = val.substring(indx).trim();
        if (!secondPart) {
            return;
        }

        inputElem.value = val.substring(0, indx).trim() + " " + secondPart;
    });

    $("#edit-profile-form").validate({
        rules: {
            name: {
                required: true,
                editProfileNameOneWordLength: true,
                editProfileNameTwoWordLengthFirstPart: true,
                editProfileNameTwoWordLengthSecondPart: true,
                pattern:
                    "(([A-Za-z0-9]{3,255})|([A-Za-z0-9]{3,123} {1}[A-Za-z0-9]{3,123}))",
            },
        },
        messages: {
            name: {
                required: "Enter Name.",
                editProfileNameOneWordLength:
                    "One word of name should contain more than 2 and less than 256 symbols.",
                editProfileNameTwoWordLengthFirstPart:
                    "First part of name should contain more than 2 and less than 124 symbols.",
                editProfileNameTwoWordLengthSecondPart:
                    "Second part of name should contain more than 2 and less than 124 symbols.",
                pattern:
                    "Name should contain only latin characters and numeric, it can be consist of two or one word.",
            },
        },
    });

    $("#profile .profile-save-btn").on(
        "click",
        function (e: JQuery.ClickEvent) {
            e.preventDefault();
            e.stopPropagation();
            $("#edit-profile-form .error").html("");

            if ($("#edit-profile-form").valid()) {
                helper.edit_profile_save_form();
            }
        }
    );

    $("#edit-profile-form input").on("change", function () {
        const self = <HTMLInputElement>this;
        self.value = self.value.trim();
        $("#edit-profile-form .error[for=total]").html("");
        $("#edit-profile-form input").valid();
    });

    $("#btnGetImage").on("click", function () {
        $("#fileUpload").prop("value", "").trigger("click");
    });

    $(".profile-container #edit-profile-link").on("click", function (e) {
        e.stopPropagation();
        e.preventDefault();

        var linkHelper = new LinkHelper();
        $.ajax({
            url: linkHelper.editProfileCheckAccess(),
            type: 'GET',
            success: function (data) {
                if (!data) {
                    location.href = linkHelper.mainPage();
                } else {
                    location.href = (<HTMLLinkElement>e.target).href;
                }
            },
            error: function () {
                location.href = linkHelper.mainPage();
            }
        });
    });

    $("#fileUpload").on("change", function () {
        const self = <HTMLInputElement>this;
        const val = self.files[0].type,
            regex = /(.*?).(jpeg|jpg|png|gif)$/;
        const size = self.files[0].size / 1024 / 1024;
        if (!regex.test(val)) {
            $("#modal-info-profile #modalBody")
                .html($("#invalid-type-dialog").html())
                .find(".invalid-type-dialog-message")
                .text("Current format is " + self.files[0].type + ".");
            $("#modal-info-profile").modal("show");
            return false;
        } else if (size > 50) {
            helper.show_alert("The maximum allowable file size is 50 MB.");
            return false;
        } else {
            helper.read_file_from_input(self, function () {
                $("#imgModal-btnSave").hide();
                $("#imgModal-btnCrop").show();
                $("#croppmodal").modal("show");
            });
        }
    });

    $("#croppmodal").on("shown.bs.modal", function () {
        helper.show_modal();
    });

    $("#imgModal-btnCrop").on("click", function () {
        helper.crop_into_img(
            <HTMLImageElement>$("#cropped-img")[0],
            function () {
                $("#imgModal-btnSave").show();
                $("#imgModal-btnCrop").hide();
            }
        );
    });

    $("#imgModal-btnSave").on("click", function () {
        helper.upload_image(<HTMLImageElement>$("#cropped-img")[0]);
    });

    $(".profile-close-btn").on("click", function () {
        const $mdl = $("#modal-info-profile");
        $mdl.find("#modalBody").html($("#close-btn-dialog-body-1").html());
        $mdl.modal("show");
    });

    $(window).on("resize", function () {
        helper.show_modal();
    });

    $("#modal-info-profile").on(
        "click",
        "#profile-close-btn-first",
        function () {
            $("#modal-info-profile")
                .find("#modalBody")
                .html($("#close-btn-dialog-body-2").html());
        }
    );

    $("#modal-info-profile").on(
        "click",
        "#profile-close-btn-second",
        function () {
            const linkHelper = new LinkHelper();
            $.ajax({
                type: "POST",
                url: linkHelper.editProfileCloseaccount(),
                contentType: false,
                processData: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader(
                        "XSRF-TOKEN",
                        <string>(
                            $(
                                '#edit-profile-form input:hidden[name="__RequestVerificationToken"]'
                            ).val()
                        )
                    );
                },
                success: function () {
                    document.location.href = linkHelper.mainPage();
                },
                error: function (err) {
                    if (err.status === 401) {
                        document.location.href = linkHelper.mainPage();
                    } else if (err.status === 404) {
                        document.location.href = linkHelper.notFoundPage();
                    }
                },
            });

            $("#modal-info-profile").modal("hide");
        }
    );
});
