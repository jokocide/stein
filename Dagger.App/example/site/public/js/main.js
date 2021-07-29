function sendToRoot() {
    console.log('moved to root');
    window.location.href = "/";
}

document.getElementById('nav-branding-container').addEventListener('click', sendToRoot);