import * as $ from "jquery";
import "bootstrap";
import { LinkHelper } from "./helpers/link-helper";

$("body")
    .off(".product-click")
    .on(
        "click.product-click",
        ".product-click",
        function (e: JQuery.ClickEvent) {
            if ((<Element>e.target).matches("a")) {
                return;
            }

            const tagId = $(e.currentTarget).attr("tag-id");
            $("#about-body").load("/products/about?tag=" + tagId,
                function (response, status, xhr) {
                    if (xhr.status === 404) {
                        $("#product-not-found-container").modal("show");
                    } else {
                        $("#about-container").modal("show");
                    }
            });
        }
    )
    .on(
        "click.product-click",
        ".open-at-telegram",
        function (e: JQuery.ClickEvent) {
            const link = $(e.currentTarget).attr("link");
            window.open(link, "_blank");
        }
    )
    .on("click.product-click", ".close-dialog-about", function () {
        $("#about-container").modal("hide");
    });
