import * as $ from "jquery";
import "bootstrap";
import * as htmx from "htmx.org";
import { LinkHelper } from "./helpers/link-helper";
import { RequestHelper } from "./helpers/request-helper";

function closeMobileMenu() {
    document.getElementById("mobile-menu").style.width = "0";
}

declare global {
    interface Window {
        toVote?: (arg1: HTMLElement, arg2: string) => Promise<void>;
        closeMobileMenu?: () => void;
    }
}

(window as Window).closeMobileMenu = closeMobileMenu;

(window as Window).toVote = async function toVote(
    element: HTMLElement,
    selectedClass: string
) {
    const data = await new RequestHelper().sendIssignedin();
    if (!data) {
        // splitted in two request because of anti-forgery validation
        $("#modal-login").modal("show");
        return;
    }

    const flag = !$(element).is("." + selectedClass),
        prodId = <string>$(element).val(),
        formData = new FormData();

    formData.append("productId", prodId);
    formData.append("voteFlag", flag.toString());

    $.ajax({
        url: "/?handler=ToVote",
        type: "POST",
        contentType: false,
        processData: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader(
                "XSRF-TOKEN",
                (<HTMLInputElement>(
                    document.getElementsByName("__RequestVerificationToken")[0]
                )).value
            );
        },
        data: formData,
        success: function () {
            successHandler($('button[value="' + prodId + '"]'), flag);
        },
        error: function (e) {
            if (e.status === 401) {
                $("#modal-login").modal("show");
            }
        },
    });

    function successHandler($elem: JQuery<HTMLElement>, flag: boolean) {
        setVoteCount($elem, flag ? 1 : -1);
        const $voteText = $elem.find(".vote-text");
        if ($voteText.length) {
            const text = flag ? "Upvoted" : "Upvote";
            $voteText.text(text);
        }

        for (let i = 0; i < $elem.length; i++) {
            const $el = $elem.eq(i);
            const attr = $el.attr("success-class");
            flag ? $el.addClass(attr) : $el.removeClass(attr);
        }
    }

    function setVoteCount($element: JQuery<HTMLElement>, num: number) {
        const countEl = $element.find(".vote-count");
        let val = parseInt(countEl.eq(0).text());
        val += num;
        $element.find(".vote-count").html(val.toString());
    }
};

$(function () {
    $(".not-auth-nav-block .hamburger, .auth-nav-block .hamburger").on(
        "click",
        function () {
            document.getElementById("mobile-menu").style.width = "100%";
        }
    );

    $(".check-tag-btn-click").on("click", async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const data = await new RequestHelper().sendIssignedin();
        $(data ? "#check-tag-modal" : "#modal-login").modal("show");
    });

    $(".logut-btn, #mobile-logout-btn").on("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        $.ajax({
            type: "POST",
            url: "/api/auth/logout",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader(
                    "XSRF-TOKEN",
                    localStorage.getItem("CSRFToken")
                );
            },
            success: function () {
                location.href = new LinkHelper().mainPage();
            },
            error: function (err) {
                if (err.status === 401) {
                    location.href = new LinkHelper().mainPage();
                }
            },
        });
    });

    $(window).on("resize", function () {
        closeMobileMenu();
    });

    htmx.on("htmx:responseError", function (e: CustomEvent) {
        if (e?.detail?.xhr?.status === 401) {
            document.location.href = new LinkHelper().mainPage();
        }
    });
});
