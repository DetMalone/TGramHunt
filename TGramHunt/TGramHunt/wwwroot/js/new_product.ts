import * as $ from "jquery";
import "jquery-validation";
import { LinkHelper } from "./helpers/link-helper";

declare global {
    interface Window {
        activateNewProductPage?: () => void;
    }
}

(window as Window).activateNewProductPage = function activateNewProductPage() {
    const productform = $("#create-prod-form");

    const validate = productform.validate({
        rules: {
            name: {
                normalizer: function (value: string) {
                    return (value || "").trim();
                },
            },
            description: {
                normalizer: function (value: string) {
                    return (value || "").trim();
                },
            },
        },
        messages: {
            category: "Please choose product category.",
            name: {
                required: "Please specify product name.",
                maxlength: "Please enter no more than 255 characters.",
            },
            tag: {
                required: "This field is required.",
                pattern: "Incorrect tag.",
            },
            description: {
                required: "Please specify product description.",
                maxlength: "Please enter no more than 2000 characters.",
            },
            cover: {
                required: "Please upload product cover.",
            },
            maker: {
                maxlength: "Please enter no more than 100 characters.",
                pattern: "Incorrect name.",
            },
        },
        onfocusout: function (element) {
            $(element).valid();
        },
    });

    productform.on("submit", function (event) {
        event.preventDefault();
        const linkHelper = new LinkHelper();

        $("#maker").val("");

        if (productform.valid()) {
            $.ajax({
                type: "POST",
                data: getFormData(),
                url: "/products/new?handler=CreateProduct",
                contentType: false,
                processData: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader(
                        "XSRF-TOKEN",
                        <string>(
                            $(
                                'input:hidden[name="__RequestVerificationToken"]'
                            ).val()
                        )
                    );
                },
                success: function () {
                    document.location.href = linkHelper.mainPage();
                },
                error: function (err) {
                    switch (err.status) {
                        case 400:
                            {
                                if (!err.responseJSON) {
                                    validate.showErrors({
                                        tag: err.responseText,
                                    });
                                    $("input#tag").trigger("focus");
                                    break;
                                }

                                const k = Object.keys(err.responseJSON);
                                let errors: { [key: string]: string } = {};

                                if (k.length === 1 && k[0] === "") {
                                    errors = { cover: "File size too large" };
                                } else {
                                    Object.entries(err.responseJSON).forEach(
                                        (error) => {
                                            errors[error[0].toLowerCase()] = (<
                                                Array<string>
                                            >error[1]).join("<br>");
                                        }
                                    );
                                }

                                validate.showErrors(errors);
                            }
                            break;

                        case 401:
                            document.location.href = linkHelper.mainPage();
                            break;

                        case 404:
                            {
                                document.location.href =
                                    linkHelper.notFoundPage();
                            }
                            break;

                        case 413:
                            {
                                const error: { [key: string]: string } = {
                                    cover: "File size too large",
                                };
                                validate.showErrors(error);
                            }
                            break;

                        default:
                            break;
                    }
                },
            });
        }
    });

    function getFormData() {
        const formData = new FormData();
        formData.append(
            "Category",
            <string>(
                $('select[name="category"] option').filter(":selected").val()
            )
        );
        formData.append("Tag", <string>$("#tag").val());
        formData.append("Name", <string>$("#name").val());
        formData.append("Description", <string>$("#description").val());
        formData.append("Cover", $("input[type=file]").prop("files")[0]);
        getMakers().forEach(function (elem) {
            formData.append("Makers[]", elem);
        });

        return formData;
    }
};

function getMakers(): Array<string> {
    const makers: Array<string> = [];
    $(".maker-name").each(function () {
        makers.push(this.textContent);
    });

    return makers;
}
