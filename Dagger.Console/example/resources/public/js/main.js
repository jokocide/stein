function sendToRoot() {
    console.log('moved to root');
    window.location.href = "/";
}

document.getElementById('branding').addEventListener('click', sendToRoot);