document.addEventListener("DOMContentLoaded", function() {
    const contentElement = document.querySelector('#Content');
    if (contentElement) {
        ClassicEditor
            .create(contentElement)
            .catch(error => {
                console.error('CKEditor 初期化エラー:', error);
            });
    }
});
