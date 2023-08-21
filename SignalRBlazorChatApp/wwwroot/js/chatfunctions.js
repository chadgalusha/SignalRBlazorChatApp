
export function scrollToBottom() {
    var element = document.getElementById('chatanchor');

    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}