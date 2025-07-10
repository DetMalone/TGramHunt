const signalMessage = "redirected",
    rediectAfterAuthToUrl = "/";

if (window !== window.top) {
    /* Iframe call to parent about redirect. */
    window.top.postMessage(signalMessage, window.location.origin);
}

declare global {
    interface Window {
        setScriptToIframe?: (
            arg1: string,
            arg2: Element,
            arg3: string,
            arg4: string
        ) => void;
    }
}

/*
IframeMain contains iframe1
*/
(window as Window).setScriptToIframe = function setScriptToIframe(
    iframeId: string,
    scriptEl: Element,
    srcRef: string,
    srcStyle: string
) {
    const iframeMain = <HTMLIFrameElement>document.getElementById(iframeId);
    iframeMain.setAttribute("src", srcRef);

    window.addEventListener(
        "message",
        function (event: MessageEvent<string>) {
            if (event.origin !== window.location.origin) {
                return;
            }

            if (event.data === signalMessage) {
                location.href = rediectAfterAuthToUrl;
            }
        },
        false
    );

    iframeMain.onload = function () {
        const iframeMainDocument = iframeMain.contentDocument,
            body = document.createElement("body"),
            library = document.createElement("script"),
            countOfScripts = 2;
        let i = 0; // js and style

        library.innerHTML = scriptEl.innerHTML;
        copyAttributes(scriptEl, library);

        library.onload = function () {
            afterCallBack(iframeMain, ++i, countOfScripts);
        };

        body.appendChild(library);

        const ibody = iframeMainDocument.body;
        ibody.parentNode.replaceChild(body, ibody);

        setStyle(
            iframeMainDocument,
            srcStyle,
            afterCallBack.bind(null, iframeMain, ++i, countOfScripts)
        );
    };
};

function afterCallBack(
    iframeMain: HTMLIFrameElement,
    counter: number,
    countOfScripts: number
) {
    if (counter % countOfScripts !== 0) {
        return;
    }

    const contentMainWindow = iframeMain.contentWindow;
    const listener = function (e: MessageEvent<string>) {
        try {
            const parsedD = JSON.parse(e.data);
            if (parsedD.event !== "ready") {
                return;
            }
        } catch (e) {
            return;
        }

        resizeIFrameToFitContent(iframeMain);
        iframeMain.removeAttribute("style");

        contentMainWindow.removeEventListener("message", listener);
    };

    contentMainWindow.addEventListener("message", listener, false);

    contentMainWindow.onunload = function () {
        iframeMain.style.visibility = "hidden";
    };
}

function resizeIFrameToFitContent(iFrame: HTMLIFrameElement) {
    const ifram1 = iFrame.contentDocument.body.querySelector("iframe");

    const resizeObserver = new ResizeObserver(() => {
        const rectIframe1 = ifram1.getBoundingClientRect();

        iFrame.width = (rectIframe1.width || ifram1.width) + "px";
        iFrame.height = (rectIframe1.height || ifram1.height) + "px";
    });

    resizeObserver.observe(ifram1);
}

function setStyle(
    iframeDocument: Document,
    srcStyle: string,
    loadFn: (this: GlobalEventHandlers, ev: Event) => void
) {
    const cssLink = document.createElement("link");
    cssLink.href = srcStyle;
    cssLink.rel = "stylesheet";
    cssLink.type = "text/css";
    cssLink.onload = loadFn;

    iframeDocument.head.appendChild(cssLink);
}

function copyAttributes(source: Element, target: Element) {
    for (const attribute of Array.from(source.attributes)) {
        target.setAttribute(
            attribute.nodeName === "id" ? "data-id" : attribute.nodeName,
            attribute.nodeValue
        );
    }
}

export {};
