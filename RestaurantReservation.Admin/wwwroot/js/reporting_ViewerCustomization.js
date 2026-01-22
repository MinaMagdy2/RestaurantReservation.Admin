window.ReportingViewerCustomization = {
    // Hide the toolbar
    onCustomizeElements: function (s, e) {
        var toolbarPart = e.GetById("dxrd-right-panel-template-base");
        var index = e.Elements.indexOf(toolbarPart);
        e.Elements.splice(index, 1);
    }
}