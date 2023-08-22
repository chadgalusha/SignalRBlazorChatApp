
export function scrollToBottom() {
    var element = document.getElementById('chatanchor');

    if (element) {
        element.scrollTo({
            top: element.scrollHeight,
            behavior: 'smooth'
        });
    }
}