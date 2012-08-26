/// <reference path="jquery-1.5.1-vsdoc.js" />
/// <reference path="RootSync.Common.js" />
/// <reference path="jquery.validate-vsdoc.js" />

var inputDialog;
var showInputDialog = function (dialogTitle, dialogText, okCallBack, cancelCallBack) {

    $('#spnInputLabel').html(dialogText);

    inputDialog = (inputDialog || $('#divInputDialog')).dialog({
        title: dialogTitle,
        autoOpen: true,
        modal: true,
        buttons: {
            "OK": function () {
                inputDialog.dialog('close');
                okCallBack($('#txtInputText').val());
            },
            "Cancel": function () {
                inputDialog.dialog('close');
                if (cancelCallBack) { cancelCallBack(); }
            }
        }
    });

}

jQuery(function () {
    registerMouseActions();
});

var registerMouseActions = function () {
    //Register context menus
    $('.clsFolderRow:has(.clsFolderName)').not('.clsParentDirectory').contextMenu({
        menu: 'mnuFolder'
    },
        function (action, element, position) {
            alert('action ' + action + ' element ' + element + ' position ' + position);
        });




    $('.clsFileRow:has(.clsFileName)').contextMenu({
        menu: 'mnuFile'
    },
    function (action, element, position) {
        var currentFile = element.find(".lnkFileName").data('fileurl');
        if (action === 'rename') {
            showInputDialog('Rename File', 'Rename:<br/>' + currentFile + '</br>To:<br/>', function (newname) {
                $.ajax({ url: "/fileactions/Rename",
                    type: "POST",
                    data: { source: currentFile, destination: newname },
                    success: function (msg) {
                        alert(msg);
                    },
                    error: ajaxError
                });
            },
            function () {
            });
        }

    });

    $('.clsFileRow').draggable({ helper: 'clone' })
                    .click(function () {
                        if ($(this).is('.ui-draggable-dragging')) {
                            return false; //don't download file
                        }
                        //alert($(this).find('a').first().data('fileurl'));

                        //$('.ui-draggable-dragging').draggable('option', 'revert', true).trigger('mouseup');
                        $(this).draggable('option', 'revert', true).trigger('mouseup');
                        return true; //download the file (or perform default action)
                    });
    $('.clsFolderRow').droppable({
        drop: function (event, ui) {
            var sourceFile = ui.draggable.find('a').first().data('fileurl');
            var destFolder = $(this).find('.clsFolderName a').first().attr('href');
            alert('codeme: move from ' + sourceFile + ' to ' + destFolder);

        }
    });
};


var relativePath = "/";

var openFolder = function (fpath) {
    if (fpath === '..') {
        relativePath = rootsync.getParentDirectory(relativePath);
    } else {
        /*if (relativePath.substring(relativePath.length - 1) !== "/") {
        relativePath = relativePath + "/" + fpath;
        } else {
        relativePath = relativePath + fpath;
        }*/
        relativePath = fpath;
    }

    //alert('sending relativePath: ' + relativePath);
    refreshDirectory(relativePath);
};

var refreshDirectory = function (rpath) {
    $.ajax({ url: "/fileactions/GetDirectoryList",
        data: { "path": rpath },
        success: function (msg) {
            $('#_index').html(msg);
            $('#MenuLocation').text(rpath == "" ? "Home" : rpath);
            registerMouseActions();
        },
        error: rootsync.ajaxError
    });
}

var newFolder = function () {

    showInputDialog('Create Folder', 'Folder Name:<br/>', function (folderName) {
        $.ajax({ url: "/fileactions/NewFolder",
            type: "POST",
            data: { Name: folderName, path: relativePath },
            success: function (msg) {
                if (msg.status === "success") {
                    if (relativePath.substring(relativePath.length - 1) === "/") {
                        relativePath += folderName;
                    } else {
                        relativePath += "/" + folderName;
                    }
                    refreshDirectory(relativePath);
                } else {
                    $(msg.responseHTML).dialog();
                }
            },
            error: rootsync.ajaxError
        });
    },
    function () {
    });
    
};

var uploadFile = function () {
    $.ajax({ url: "/fileactions/UploadFile?path=" + encodeURIComponent(relativePath),
        success: function (msg) {
            $(msg).dialog({
                autoOpen: true,
                title: "Upload File"
            });
        },
        error: rootsync.ajaxError
    });
};
//FileActions/UploadFile?path=@ViewBag.path.Replace(" ", "%20")
//<a class="openDialog" data_dialog_id="createDialog" data_dialog_title="New Folder" href="">