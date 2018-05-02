(function ($, window, codeconsole, ace, undefined)
{
    $(function ()
    {
        var editor = $('#Editor');
        var terminalCode = ace.edit('terminalCode');
        terminalCode.setTheme('ace/theme/monokai');
        terminalCode.session.setMode('ace/mode/csharp');
        terminalCode.setShowPrintMargin(false);
        terminalCode.session.setValue(editor.val());
        terminalCode.session.on("change", function () { 
            console.log('In ace editor session change');
            editor.val(terminalCode.session.getValue());
            scForm.postRequest("", "", "", "codeconsole:codemodified()");
        });
        terminalCode.setOptions({
            fontSize: "13pt"
        });

        codeconsole.resizeEditor = function () {
            terminalCode.resize();
        };

        codeconsole.appendOutput = function (outputToAppend) {
            var decoded = $("<div/>").html(outputToAppend).text();
            $("#ScriptResultCode").append(decoded);
            $("#Result").scrollTop($("Result")[0].scrollHeight);
        }

        $(window).on('resize', function () {
            codeconsole.resizeEditor();
        }).trigger('resize');

        function setFocusOnConsole() {
            $("body").focus();
            $(terminalCode).focus();
            var count = terminalCode.session.getLength();
            terminalCode.gotoLine(7, terminalCode.session.getLine(7).length);
            ("WebForm_AutoFocus" in this) && WebForm_AutoFocus && WebForm_AutoFocus("terminalCode");
        }

        window.addEventListener("focus", function (event) {
            setFocusOnConsole();
        }, false);

        window.parent.focus();
        window.focus();

        function registerEventListenersForRibbonButtons() {
            console.log('initialize');
            [].forEach.call(document.querySelectorAll('.scRibbonToolbarSmallGalleryButton, .scRibbonToolbarLargeComboButtonBottom'), function (div) {
                div.addEventListener("click", function () {
                    clearTimeout(typingTimer);
                });
            });

            [].forEach.call(document.querySelectorAll('.scRibbonNavigatorButtonsGroupButtons > a'), function (div) {
                div.addEventListener("click", function () {
                    codeconsole.updateRibbon();
                });
            });
        }

        registerEventListenersForRibbonButtons();

        var typingTimer;

        codeconsole.updateRibbon = function () {
            if (!terminalCode.getReadOnly()) {
                scForm.postRequest("", "", "", "codeconsole:updateribbon(modified=" + !terminalCode.session.getUndoManager().isClean() + ")");
                registerEventListenersForRibbonButtons();
            }
        }

        codeconsole.updateRibbonNeeded = function () {
            clearTimeout(typingTimer);
            var timeout = 2000;
            if (document.querySelector('.scGalleryFrame') != null) {
                var timeout = 20;
            }
            typingTimer = setTimeout(codeconsole.updateRibbon, timeout);
        }

        var posx = $("#PosX");
        var posy = $("#PosY");
        $("#terminalCode").on("keyup mousedown", function () {
            var position = terminalCode.getCursorPosition();
            posx.text(position.column);
            posy.text((position.row + 1));
            codeconsole.updateRibbonNeeded();
        }).trigger('keyup');
    });    
}(jQuery, window, window.codeconsole = window.codeconsole || {}, window.ace = window.ace || {}));