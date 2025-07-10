import * as $ from "jquery";

declare global {
    interface Window {
        addMaker?: () => void;
        previewFile?: () => void;
    }
}

(window as Window).addMaker = function addMaker() {
    const maker = $("#maker"),
        makerBtn = $("#add-maker-btn"),
        makers = $("#makers-list");

    if (
        maker.val() &&
        $("#create-prod-form").data("validator").element("#maker")
    ) {
        const elem = $(
            (<HTMLTemplateElement>$("#maker-template")[0]).content
        ).clone(true);
        elem.find(".maker-name").text(<string>maker.val());
        makers.append(elem);
        makers.last().on("click", (e: JQuery.ClickEvent) => {
            removeMaker(e);
        });
        maker.val("");

        if (makers.children().length === 6) {
            makerBtn.prop("disabled", true);
        }
    }
};

function removeMaker(event: JQuery.ClickEvent) {
    const origEvent = event.originalEvent;
    const path: EventTarget[] =
        (<{ path: EventTarget[] }>(<unknown>origEvent)).path || // not standart property
        (origEvent.composedPath && origEvent.composedPath());

    if (path) {
        const delElem = path.find(
            (x) =>
                x instanceof Element &&
                x.classList &&
                x.classList.contains("maker")
        );

        if (
            delElem &&
            $(window).width() < 576 ===
                ($(event.target).prop("tagName") === "I")
        ) {
            (<Element>delElem).remove();
            $("#add-maker-btn").prop("disabled", false);
        }
    }
}

(window as Window).previewFile = function previewFile() {
    const blob = $("input[type=file]").prop("files")[0];
    const preview = $(".cover-preview");
    const errField = $("label[for='cover']");

    const fileReader = new FileReader();

    fileReader.onloadend = function (e: ProgressEvent<FileReader>) {
        const arr = new Uint8Array(<ArrayBuffer>e.target.result).subarray(0, 4);
        let header = "";
        for (const element of Array.from(arr)) {
            header += element.toString(16);
        }

        let text = "";
        if (
            !(
                blob.size > 0 &&
                Number.parseFloat((blob.size / (1024 * 1024)).toFixed(2)) <= 10
            )
        ) {
            text = "Picture larger than 10MB.";
        } else if (!mimeType(header)) {
            text = "Unsupported image format.";
        } else {
            showPreview(blob, preview);
            errField.hide();
            return;
        }

        errField.text(text);
        errField.show();
        $("#cover").val(null);
        preview.attr("src", "/content/images/photo.png");
    };

    fileReader.readAsArrayBuffer(blob);
};

function showPreview(blob: Blob, preview: JQuery) {
    const reader = new FileReader();

    reader.onloadend = function () {
        preview.attr("src", <string>reader.result);
    };

    reader.readAsDataURL(blob);
}

function mimeType(headerString: string) {
    let allowedType;
    switch (headerString) {
        case "89504e47": // png
        case "ffd8ffe0": // jpg/jpeg
        case "ffd8ffe1": // jpg/jpeg
        case "ffd8ffe2": // jpg/jpeg
            allowedType = true;
            break;
        default:
            allowedType = false;
            break;
    }

    return allowedType;
}
