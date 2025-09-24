window.fileManagerMud = (function () {
    let dotnetRef = null;

    function initContextMenu(ref) {
        dotnetRef = ref;
        window.addEventListener("click", () => {
            if (dotnetRef) dotnetRef.invokeMethodAsync("CloseContextMenu");
        });
    }

    function bindContextMenuClose() {
        // Called after showing the custom menu; clicking elsewhere closes it (handled above)
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

    async function openUploadDialog(ref) {
        // You can later replace by a MudDialog. For now, let the Blazor side open its own dialog or use input file.
        // This stub exists so you can wire a custom modal later if desired.
        // No-op here to keep JS minimal.
    }

    return {
        initContextMenu,
        bindContextMenuClose,
        prompt,
        saveAsBytes,
        openUploadDialog
    };
})();
