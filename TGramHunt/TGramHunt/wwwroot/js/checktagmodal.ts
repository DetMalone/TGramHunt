import * as $ from "jquery";
import "jquery-validation";
import { LinkHelper } from "./helpers/link-helper";

$(function () {
    const checktagform = $("#check-tag-form");
    $("#check-tag-form")
        .find("#producttag")
        .on("keyup", function (e) {
            const target = <HTMLInputElement>e.target;
            addSymbolToProdTag(target);
        });

    checktagform.validate({
        messages: { producttag: { pattern: "Incorrect tag." } },
    });

    checktagform.on("submit", function (event) {
        event.preventDefault();

        if (checktagform.valid()) {
            checkTag(localStorage.getItem("CSRFToken"));
        }
    });
});

function addSymbolToProdTag(target: HTMLInputElement): string {
    if (!target) {
        return;
    }

    let val = target.value;
    if (!!val && !val.startsWith("@")) {
        val = "@" + val;
        target.value = val;
    }

    return val;
}

function checkTag(token: string) {
    const linkHelper = new LinkHelper(),
        tag = addSymbolToProdTag(<HTMLInputElement>$("#producttag")[0]);

    const data = JSON.stringify(tag);

    //first call checks if we already have a tag in products, second call checks if tag exists in Telegram
    const checkTagInDB = $.ajax({
        type: "POST",
        url: "/api/Product/CheckByTag",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", token);
        }
    });

    checkTagInDB.done(function (res) {
        if (res) {
            const errLabel = $("#producttag-error");
            errLabel.text("This product already exists");
            errLabel.show();
        } else {
            if ($('#verifyTagChkbox').prop('checked')) {
                checkTagInTg(data, token);
            } else {
                document.location.href = linkHelper.productsNew(tag);
            }
        }
    });

    checkTagInDB.fail(function (err) {
        if (err.status === 401) {
            document.location.href = linkHelper.mainPage();
        } else if (err.status === 404) {
            document.location.href = linkHelper.notFoundPage();
        }
    });


    function checkTagInTg(data: string, token: string) {

        const linkHelper = new LinkHelper(),
            tag = addSymbolToProdTag(<HTMLInputElement>$("#producttag")[0]);

        const checkTagExistence = $.ajax({
            type: "POST",
            url: "/api/Product/CheckTagInfo",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN", token);
            }
        });

        checkTagExistence.done(function (res) {
            if (res) {
                document.location.href = linkHelper.productsNew(tag);
            } else {
                const errLabel = $("#producttag-error");
                errLabel.text("Product is private or does not exist");
                errLabel.show();
            }
        });

        checkTagExistence.fail(function (err) {
            if (err.status === 401) {
                document.location.href = linkHelper.mainPage();
            } else if (err.status === 404) {
                document.location.href = linkHelper.notFoundPage();
            }
        });
    }
}
