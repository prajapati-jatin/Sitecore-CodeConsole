(function ($, window, codeconsole, ace, undefined) {
    $(function () {
        var editor = $('#Code');
        var terminalCode = ace.edit('terminalCode');
        terminalCode.setTheme('ace/theme/monokai');
        terminalCode.session.setMode('ace/mode/csharp');
        terminalCode.session.setValue(editor.val());
        terminalCode.session.on("change", function () {
            editor.val(terminalCode.session.getValue());
        })
    });
}(jQuery, window, window.codeconsole = window.codeconsole || {}, window.ace = window.ace || {}));