/// <reference path="jquery-1.5.1-vsdoc.js" />
/// <reference path="jquery.validate-vsdoc.js" />

$.ajaxSetup({ cache: false });

$(function () {
    $('.jqButton').button();
});

(function (context, undefined) {
    var rootsync = context.rootsync = context.rootsync || {};

    rootsync.ajaxError = function (a, b, c) {
        alert('An AJAX error has occurred: ' + a.responseText);
    };

    rootsync.getParentDirectory = function (dir) {
        var dirs = dir.split('/');

        if (dirs.length < 2) { return ''; }

        if (dirs[0] === "") {
            dirs = dirs.slice(1, dirs.length);
        }
        if (dirs.length === 1) { return ''; } // the first element was empty and there was only one left
        while (dirs.length > 1 && dirs[dirs.length - 1] === "") {
            dirs = dirs.slice(0, dirs.length - 1);
        }
        if (dirs.length === 1) { return ''; } //only one element after removing trailing empty elements.
        if (dirs.length > 2) {
            return dirs[dirs.length - 2];
        }
        return dirs[0];
    }

})(window);
