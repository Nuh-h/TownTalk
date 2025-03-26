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


    //Posts pagination

    window.loadPosts = function (page) {
        const urlParams = new URLSearchParams(window.location.search);
        urlParams.set("page", page);

        const queryString = urlParams.toString();

        fetch(`/Posts/GetPosts?${queryString}`)
            .then(response => response.text())
            .then(data => {
                // Replace the PostList and Pagination parts with new content
                document.getElementById("resultsContainer").innerHTML = data;

                const reactionsModule = new ReactionsModule();
                reactionsModule.init();
            });
    }
});
