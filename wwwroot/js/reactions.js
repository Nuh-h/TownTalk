$(document).ready(function() {
    //Reactions
    $(function () {
        let $popover;
        function initializePopovers() {
            $('[data-bs-toggle="popover"]').popover({
                trigger: 'click',
                placement: 'top',
                html: true,
                content: function () {
                    const reactionPopoverContent = $popover.siblings('.reactionPopoverContent')
                    console.log({reactionPopoverContent, currentElement: $popover})
                    return reactionPopoverContent.clone().show();
                }
            });
        }

        initializePopovers();

        $(document).on('click', '.reaction-btn', function () {
            $popover = $(this);
            const isExpanded = $popover.attr('aria-expanded') === 'true';

            // Hide other popovers
            $('[data-bs-toggle="popover"]').popover('hide');

            if (isExpanded) {
                $popover.popover('hide');
                $popover.attr('aria-expanded', 'false');
            } else {
                $popover.popover('show');
                $popover.attr('aria-expanded', 'true');
            }
        });

        // Handle selection of reactions from the popover
        $(document).on('click', '.reactionPopoverContent.reaction', function () {
            const reactionType = $(this).data('reaction-type');
            const reactionId = $(this).data('reaction-id'); // Get the reaction ID
            const postId = $(this).parent().data('post-id');

            // Get the current active reaction
            const $reactionContainer = $('.reaction-container[data-post-id="' + postId + '"]');
            const currentReaction = $reactionContainer.data('active-reaction');

            // Check if the clicked reaction is the current active reaction
            if (reactionType === currentReaction) {
                submitReaction(reactionType, postId, reactionId); // Undo reaction
            } else {
                submitReaction(reactionType, postId); // Submit new reaction
            }

            // Hide popover after selection
            $('[data-bs-toggle="popover"]').popover('hide');
        });

        // Handle selection of reactions from the popover
        $(document).on('click', '.reaction.btn', function () {
            const reactionType = $(this).data('reaction-type');
            const postId = $(this).parent().data('post-id');

            // Get the current active reaction
            const $reactionContainer = $('.reaction-container[data-post-id="' + postId + '"]');
            const currentReaction = $reactionContainer.data('active-reaction');

            // Check if the clicked reaction is the current active reaction
            if (reactionType === currentReaction) {
                // Undo reaction by passing postId and reactionType
                submitReaction(postId, reactionType, true);
            } else {
                // Submit new reaction
                submitReaction(postId, reactionType, false);
            }

            $('[data-bs-toggle="popover"]').popover('hide');
        });

        /**
         * To create or undo a reaction
         * @param {*} postId
         * @param {*} reactionType
         * @param {boolean} isDeleting
         */
        function submitReaction(postId, reactionType, isDeleting) {
            const $reactionContainer = $('.reaction-container[data-post-id="' + postId + '"]');

            if (isDeleting) {
                $.ajax({
                    url: `/Reactions/Delete`,
                    type: 'DELETE',
                    contentType: 'application/json',
                    data: JSON.stringify({ PostId: postId, Type: reactionType }),
                    success: function () {
                        updateReactions(postId);
                    },
                    error: function () {
                        alert('Error processing reaction. Please try again.');
                    }
                });
            } else {
                $.ajax({
                    url: `/Reactions/Create`,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify({ PostId: postId, Type: reactionType }),
                    success: function () {
                        updateReactions(postId);
                    },
                    error: function () {
                        alert('Error processing reaction. Please try again.');
                    }
                });
            }
        }

        function updateReactions(postId) {
            $.ajax({
                url: `/Posts/GetReactions/${postId}`,
                type: 'GET',
                success: function (data) {
                    $('.reaction-container[data-post-id="' + postId + '"]').replaceWith(data);
                    initializePopovers();
                },
                error: function () {
                    alert('Error loading updated reactions. Please refresh the page.');
                }
            });
        }


        // Close popover when clicking outside of it
        $(document).on('click', function (e) {
            if (!$(e.target).closest('.reaction-container').length) {
                $('[data-bs-toggle="popover"]').popover('hide');
            }
        });

    });
})