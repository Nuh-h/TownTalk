$(document).ready(function () {
    const scrollToTopBtn = $('#scrollToTopBtn');

    $(window).scroll(function () {
        if ($(this).scrollTop() > 200) {
            scrollToTopBtn.fadeIn();
        } else {
            scrollToTopBtn.fadeOut();
        }
    });

    scrollToTopBtn.on('click', function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
    });
});
