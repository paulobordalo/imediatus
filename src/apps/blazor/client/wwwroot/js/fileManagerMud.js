window.fileManagerMud = (function () {
    let dotnetRef = null;

    function initContextMenu(ref) {
        dotnetRef = ref;
        window.addEventListener("click", () => {
            if (dotnetRef) dotnetRef.invokeMethodAsync("CloseContextMenu");
        });
    }

    function bindContextMenuClose() {
        // Reserved for future logic.
    }

    function prompt(message) {
        return window.prompt(message);
    }

    function saveAsBytes(filename, contentType, base64Data) {
        try {
            const bytes = typeof base64Data === "string" ? Uint8Array.from(atob(base64Data), c => c.charCodeAt(0)) : base64Data;
            const blob = new Blob([bytes], { type: contentType || "application/octet-stream" });
            const url = URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = filename || "download";
            document.body.appendChild(a);
            a.click();
            a.remove();
            URL.revokeObjectURL(url);
        } catch (e) {
            console.error("saveAsBytes failed", e);
        }
    }

    // New unified download function:
    // Parameters:
    //  fileName: desired download file name
    //  url: (optional) direct/SAS url. If provided it is preferred.
    //  contentType: MIME type when using base64 fallback
    //  base64Data: (optional) base64 payload if no url
    function downloadFile(fileName, url, contentType, base64Data) {
        try {
            if (url) {
                const a = document.createElement("a");
                a.href = url;
                a.download = fileName || "";
                document.body.appendChild(a);
                a.click();
                a.remove();
                return;
            }
            if (base64Data) {
                const bytes = Uint8Array.from(atob(base64Data), c => c.charCodeAt(0));
                const blob = new Blob([bytes], { type: contentType || "application/octet-stream" });
                const objectUrl = URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = objectUrl;
                a.download = fileName || "download";
                document.body.appendChild(a);
                a.click();
                a.remove();
                URL.revokeObjectURL(objectUrl);
            }
        } catch (e) {
            console.error("downloadFile failed", e);
        }
    }

    async function openUploadDialog(ref) {
        // Stub for future upload UI
    }

    return {
        initContextMenu,
        bindContextMenuClose,
        prompt,
        saveAsBytes,
        downloadFile,
        openUploadDialog
    };
})();
