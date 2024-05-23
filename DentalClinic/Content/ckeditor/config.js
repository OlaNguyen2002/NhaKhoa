/**
 * @license Copyright (c) 2003-2020, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    //config.language = 'fr';
    //config.uiColor = '#AADC6E';
    config.fullPage = false;
    config.allowedContent = true;

    //config.extraPlugins = 'image2';
    //config.filebrowserBrowseUrl = '/Content/ckfinder/ckfinder.html';
    //config.filebrowserImageBrowseUrl = '/Content/ckfinder/ckfinder.html?Type=Images';
    //config.filebrowserUploadUrl = '/Content/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
    //config.filebrowserImageUploadUrl = '/Content/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images';
    //config.filebrowserWindowWidth = '1000';
    //config.filebrowserWindowHeight = '700';

    config.extraPlugins = 'image2';
    config.filebrowserBrowseUrl = '../../Content/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '../../Content/ckfinder.html?Type=Images';
    config.filebrowserUploadUrl = '../../Content/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = '../../Content/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images';
    config.filebrowserWindowWidth = '1000';
    config.filebrowserWindowHeight = '700';
};
