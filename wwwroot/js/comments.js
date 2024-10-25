$(document).ready(function () {

    // Toggle comments visibility
    $(document).on('click', '[data-toggle="collapse"]', function () {
        var target = $(this).data('target');
        $(target).toggleClass('show');
    })

    // Comment Handling
    $(document).on('submit', '.comment-form', function (e) {
        e.preventDefault();

        var form = $(this);
        $.ajax({
            type: 'POST',
            url: form.attr('action'),
            data: form.serialize(),
            success: function (data) {
                if (data.success) {
                    var newCommentHtml = `
                                <div style="background: #00000059; padding: 1rem; border-radius: 0.3em;">
                                    <strong>${data.userDisplayName}</strong>
                                    <p class="text-muted"><small>Posted on ${data.createdAt}</small></p>
                                    <p>${data.content}</p>
                                    <button class="btn btn-link btn-sm text-muted mx-0 p-0 reply-button" data-parent-comment-id="${data.id}" data-reply-form-id="${data.id}">Reply</button>
                                    <button class="btn btn-link btn-sm text-muted mx-0 p-0 delete-comment" data-comment-id="${data.id}">Delete</button>
                                    <div class="reply-form" style="display:none;" id="${data.id}">
                                        <form class="comment-form" method="post" action="/Comments/Create">
                                            <input type="hidden" name="PostId" value="${data.postId}">
                                            <input type="hidden" name="ParentCommentId" value="${data.id}">
                                            <div class="input-group mt-2">
                                                <input type="text" class="form-control" name="Content" placeholder="Add a reply..." required="">
                                                <div class="input-group-append">
                                                    <button class="btn btn-primary" type="submit">Reply</button>
                                                </div>
                                            </div>
                                        </form>
                                    </div>
                                    <div class="replies-list mt-2"></div>
                                </div>
                            `;

                    var repliesContainer = $('[data-comment-id="' + data.parentCommentId + '"] .replies-list');

                    repliesContainer.prepend(newCommentHtml);

                    form[0].reset();
                } else {
                    alert('Error creating comment. Please try again.');
                }
            },
            error: function () {
                alert('There was an error submitting your comment. Please try again.');
            }
        });
    });

    // Toggle reply form visibility
    $(document).on('click', '.reply-button', function () {
        var replyFormId = $(this).data('reply-form-id');
        $('#' + replyFormId).toggle(); // Show or hide the corresponding reply form
    });


    // Delete Comment Handling
    $(document).on('click', '.delete-comment', function () {
        var commentId = $(this).data('comment-id');
        $.ajax({
            type: 'POST',
            url: '/Comments/Delete/' + commentId,
            data: { id: commentId },
            success: function (response) {
                if (response.success) {
                    $('[data-comment-id="' + response.id + '"]').remove();
                } else {
                    alert('Error deleting the comment. Please try again.');
                }
            },
            error: function () {
                alert('There was an error deleting the comment. Please try again.');
            }
        });
    });

});