const articleId = document.getElementById("articleId").value;

async function loadComments() {
    let response = await fetch(`/api/comments/${articleId}`);
    let comments = await response.json();

    let html = "";
    comments.forEach(c => {
        html += `<p><strong>${c.author}</strong> (${new Date(c.postDate).toLocaleString()})<br>${c.content}</p><hr>`;
    });

    document.getElementById("comment-list").innerHTML = html;
}

async function postComment() {
    const author = document.getElementById("comment-author").value || "匿名";  // 名前が空なら「匿名」
    const content = document.getElementById("comment-content").value.trim();

    if (!content) {
        alert("コメント内容を入力してください");
        return;
    }

    const response = await fetch("/api/comments", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ articleId, author, content })
    });

    if (response.ok) {
        document.getElementById("comment-content").value = "";  // コメント入力欄をクリア
        loadComments();  // コメントリストを再読み込み
    } else {
        alert("コメントの投稿に失敗しました");
    }
}

// 初回ロード時にコメントを表示
document.addEventListener("DOMContentLoaded", () => {
    loadComments();
});
