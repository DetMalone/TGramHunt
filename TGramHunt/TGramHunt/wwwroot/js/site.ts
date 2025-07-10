import * as $ from "jquery";
import "jquery-validation";

$(function () {
    $.validator.addMethod(
        "exist_letters",
        function (value) {
            return /[a-zA-Z]/.test(value);
        },
        "Bad value."
    );
});
