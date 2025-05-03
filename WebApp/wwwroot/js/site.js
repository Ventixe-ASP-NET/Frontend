document.addEventListener('DOMContentLoaded', function () {
    const menuBtn = document.querySelector('#menu-btn');
    const menu = document.querySelector('#aside-menu');

    menuBtn.addEventListener('click', function () {
        menu.classList.toggle('show');
    });
});