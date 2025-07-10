import * as $ from "jquery";

class RequestHelper {
    async sendIssignedin() {
        return $.ajax({
            url: "/api/auth/issignedin",
            type: "POST",
        });
    }
}

export { RequestHelper };
