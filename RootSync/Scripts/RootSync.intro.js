/// <reference path="RootSync.Common.js" />
/// <reference path="jquery-1.5.1-vsdoc.js" />
/// <reference path="jquery.validate-vsdoc.js" />

(function (context, undefined) {
    var intro = window.intro = window.intro || {};
    intro.signInDialog = undefined;
    intro.registerDialog = undefined;

    intro.register_cancel_OnClick = function () {
        if (intro.registerDialog) {
            intro.registerDialog.dialog('close');
        }
    };
    intro.signin_cancel_OnClick = function () {
        if (intro.signInDialog) {
            intro.signInDialog.dialog('close');
        }
    };

    intro.signin_initForm = function () {
        $('#frmSignIn').submit(function () {
            if ($(this).valid()) {
                $.ajax({ url: this.action,
                    async: false, //force browser to wait for this action to complete
                    type: this.method,
                    data: $(this).serialize(),
                    success: function (result) {
                        if (result.status === "failure") {
                            //This works because jQuery will strip the parent (the form)
                            //If we replace the form we have, we'll lose our subscription to events.
                            $('#frmSignIn').html($(result.responseHTML).html());
                        } else if (result.status === "error") {
                            $(result.responseHTML).dialog();
                        } else {
                            window.location.href = intro.filesAction;
                        }
                    },
                    error: function (a, b, c) {
                        rootsync.ajaxError(a, b, c);
                    }
                });
            }
            return false;
        });
    };

    intro.register_initForm = function () {
        $('#frmRegister').submit(function () {
            if ($(this).valid()) {
                $.ajax({ url: this.action,
                    async: false, //force browser to wait for this action to complete
                    type: this.method,
                    data: $(this).serialize(),
                    success: function (result) {
                        $("#ProgressDialog").dialog('close');
                        if (result.status === "failure") {
                            //This works because jQuery will strip the parent (the form)
                            //If we replace the form we have, we'll lose our subscription to events.
                            $('#frmRegister').html($(result.responseHTML).html());
                        } else {
                            window.location.href = intro.filesAction;
                        }
                    },
                    error: function (a, b, c) {
                        $("#ProgressDialog").dialog('close');
                        rootsync.ajaxError(a, b, c);
                    }
                });
            }
            return false;
        });
    };

    intro.showRegister = function () {
        var x = $(signinlinks).position().left - $(signinlinks).width() / 2;
        var y = $(signinlinks).position().top + $(signinlinks).height() * 1.5;
        if (!intro.registerDialog) {
            $.ajax({ url: "/account/register",
                success: function (msg) {
                    intro.registerDialog = $('<div>' + msg + '</div>').dialog({
                        modal: true,
                        title: "Create An Account",
                        draggable: false,
                        position: [x, y],
                        closeOnEscape: true,
                        resizable: false,
                        autoOpen: false
                    });
                    intro.register_initForm();
                    intro.registerDialog.dialog('open');
                },
                error: rootsync.ajaxError
            });
        } else {
            intro.registerDialog.dialog('open');
        }
    };
    intro.showSignIn = function () {
        var x = $(signinlinks).position().left - $(signinlinks).width() / 2;
        var y = $(signinlinks).position().top + $(signinlinks).height() * 1.5;
        if (!intro.signInDialog) {
            $.ajax({ url: "/account/signin",
                success: function (msg) {
                    intro.signInDialog = $('<div>' + msg + '</div>').dialog({
                        modal: true,
                        title: "Sign In",
                        draggable: false,
                        position: [x, y],
                        closeOnEscape: true,
                        resizable: false,
                        autoOpen: false
                    });
                    intro.signin_initForm();
                    intro.signInDialog.dialog('open');
                },
                error: rootsync.ajaxError
            });
        } else {
            intro.signInDialog.dialog('open');
        }
    };


    $(function () {

        // Initialize progress dialog ...
        $("#ProgressDialog").dialog({
            autoOpen: false,
            draggable: false,
            modal: true,
            resizable: false,
            title: "Validating Credentials...",
            closeOnEscape: false,
            open: function () { $(".ui-dialog-titlebar-close").hide(); } // Hide close button
        });

        // Initialize success dialog ...
        $("#SuccessDialog").dialog({
            autoOpen: false,
            draggable: false,
            modal: true,
            resizable: false,
            title: "Sign In Status",
            buttons: [
        {
            text: "OK",
            click: function () { $(this).dialog("close"); }
        }]
        });

        $('form').submit(function () { alert('this fired'); });


    });
})(window);