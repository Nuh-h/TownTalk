class ReactionsModule {
    reactionButtonSelector = ".reaction-btn";
    reactionPopoverContentSelector = ".reactionPopoverContent";
    reactionContainerSelector = ".reaction-container";
    popoverTriggerSelector = '[data-bs-toggle="popover"]';
    reactionBtnSelector = '.reaction.btn';
    $popover = null

    constructor() {
    }

    init() {
        this.initializePopovers();
        this.setupEventListeners();
    }

    setupEventListeners() {
        $(document).on('click', this.reactionButtonSelector, (e) => {
            this.$popover = $(e.currentTarget);
            this.togglePopover();
        });

        $(document).on('click', this.reactionBtnSelector, (e) => {
            const $target = $(e.currentTarget);
            const reactionType = $target.data('reaction-type');
            const postId = $target.closest(this.reactionContainerSelector).data('post-id');
            const currentReaction = this.getCurrentReaction(postId);

            if (reactionType === currentReaction) {
                this.submitReaction(postId, reactionType, true);
            } else {
                this.submitReaction(postId, reactionType);
            }

            $(this.popoverTriggerSelector).popover('hide');
        });

        $(document).on('click', (e) => {
            if (!$(e.target).closest(this.reactionContainerSelector).length) {
                $(this.popoverTriggerSelector).popover('hide');
            }
        });
    }

    togglePopover() {
        const isExpanded = this.$popover.attr('aria-expanded') === 'true';
        $(this.popoverTriggerSelector).popover('hide');

        if (isExpanded) {
            this.$popover.popover('hide');
            this.$popover.attr('aria-expanded', 'false');
        } else {
            this.$popover.popover('show');
            this.$popover.attr('aria-expanded', 'true');
        }
    }

    initializePopovers() {
        $(this.popoverTriggerSelector).popover({
            trigger: 'click',
            placement: 'top',
            html: true,
            content: () => {
                const reactionPopoverContent = this.$popover.siblings(this.reactionPopoverContentSelector);
                return reactionPopoverContent.clone().show();
            }
        });
    }

    getCurrentReaction(postId) {
        const $reactionContainer = $(`${this.reactionContainerSelector}[data-post-id="${postId}"]`);
        return $reactionContainer.data('active-reaction');
    }

    submitReaction(postId, reactionType, isDeleting) {
        const url = isDeleting ? `/Reactions/Delete` : `/Reactions/Create`;
        const method = isDeleting ? 'DELETE' : 'POST';

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify({ PostId: postId, Type: reactionType }),
            success: () => {
                this.updateReactions(postId);
            },
            error: () => {
                alert('Error processing reaction. Please try again.');
            }
        });
    }

    updateReactions(postId) {
        $.ajax({
            url: `/Posts/GetReactions/${postId}`,
            type: 'GET',
            success: (data) => {
                $(`${this.reactionContainerSelector}[data-post-id="${postId}"]`).replaceWith(data);
                this.initializePopovers();
            },
            error: () => {
                alert('Error loading updated reactions. Please refresh the page.');
            }
        });
    }
}

// Initialize the ReactionsModule
$(document).ready(function () {
    const reactionsModule = new ReactionsModule();
    reactionsModule.init();

    // Example of accessing updateReactions from outside
    window.updateReactionsForPost = (postId) => {
        reactionsModule.updateReactions(postId);
    };
});
