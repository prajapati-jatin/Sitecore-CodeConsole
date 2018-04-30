jQuery(document).ready(function () {
    console.log('ok loaded codeconsole.js');
    window.editorUsingStatements = ace.edit('editorUsingStatement');

    initAceEditor(window.editorUsingStatements);

    function initAceEditor(editor) {
        editor.setTheme('ace/theme/monokai');
        editor.session.setMode('ace/mode/csharp');
    }
});