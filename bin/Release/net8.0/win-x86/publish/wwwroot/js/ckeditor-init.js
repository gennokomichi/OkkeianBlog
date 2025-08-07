document.addEventListener("DOMContentLoaded", function() {
    const contentElement = document.querySelector("#Content");
    if (contentElement) {
        ClassicEditor
            .create(contentElement, {
                ckfinder: {
                    uploadUrl: "/Admin/UploadImage" // 画像アップロード用エンドポイント
                },
                toolbar: [
                    'heading', '|', 'bold', 'italic', 'fontSize', '|', 'link', 'blockQuote', 'insertTable', 'mediaEmbed', 'imageUpload', '|', 'undo', 'redo'
                ],
                fontSize: {
                    options: [
                        'tiny', // 小さいフォント
                        'small',
                        'default', // デフォルトフォント
                        'big',
                        'huge' // 大きいフォント
                    ],
                    supportAllValues: true // カスタムフォントサイズをサポート
                },
                mediaEmbed: {
                    previewsInData: true // 埋め込んだ動画をプレビュー可能にする
                }
            })
            .catch(error => {
                console.error("CKEditor 初期化エラー:", error);
            });
    }
});