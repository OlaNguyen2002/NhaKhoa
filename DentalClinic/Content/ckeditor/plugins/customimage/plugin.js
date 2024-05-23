

CKEDITOR.plugins.add('customimage', {
    icons: 'customimage',
    init: function (editor) {
        function insertImage(url) {
            editor.execCommand('insertImage', {
                url: url,
                alt: '',
                title: '',
                border: '0'
            });
        }

        editor.ui.addButton('CustomImage', {
            label: 'Insert Custom Image',
            command: 'customimage',
            toolbar: 'insert',
            icon: 'image'
        });

        editor.addCommand('customimage', {
            exec: function (editor) {

                //editor.execCommand('insertImage', { source: 'https://images.pexels.com/photos/268533/pexels-photo-268533.jpeg?cs=srgb&dl=pexels-pixabay-268533.jpg&fm=jpg' });

                var url = "https://images.pexels.com/photos/268533/pexels-photo-268533.jpeg?cs=srgb&dl=pexels-pixabay-268533.jpg&fm=jpg";
                if (url) {
                    var html = '<img style="max-width:100%;height:auto;margin:0 auto;" src="' + url + '" alt="" title="" />';
                    editor.insertHtml(html);
                }
            }
        });
    }
});