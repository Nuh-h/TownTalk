import $ from 'jquery';
import * as bootstrap from "bootstrap";
import { Popover } from 'bootstrap';

/**
 * Manages the UI and AJAX logic for post reactions (like, love, etc.) in the application.
 *
 * This class handles:
 * - Initializing and managing Bootstrap popovers for reaction buttons.
 * - Handling user interactions for adding, removing, and updating reactions.
 * - Sending AJAX requests to create or delete reactions on the server.
 * - Updating the UI with the latest reaction data after changes.
 *
 * Usage:
 * Instantiate this class to enable reaction functionality on the page.
 *
 * @example
 * const reactions = new Reactions();
 *
 * @remarks
 * - Relies on Bootstrap's Popover component and jQuery for AJAX and DOM manipulation.
 * - Exposes a global function `updateReactionsForPost(postId)` for external updates.
 *
 * @public
 */
class Reactions {

    reactionButtonSelector = ".reaction-btn";
    reactionPopoverContentSelector = ".reactionPopoverContent";
    reactionContainerSelector = ".reaction-container";
    popoverTriggerSelector = '[data-bs-toggle="popover"]';
    reactionBtnSelector = '.reaction.btn';
    $popover?: HTMLElement;

    constructor() {
        this.init()
    }

    init() {

        this.initializePopovers();
        this.setupEventListeners();

        (window as any).updateReactionsForPost = (postId: string) => {
            this.updateReactions(postId);
        };
    }

    setupEventListeners() {
        // Click on popover trigger button
        document.addEventListener('click', (e) => {
            const target = e.target as HTMLElement;

            // Match reaction button (popover trigger)
            if (target.closest(this.reactionButtonSelector)) {
                this.$popover = target.closest(this.popoverTriggerSelector) as HTMLElement;
                this.togglePopover();
            }

            // Match inside reaction menu (e.g. like, love, etc.)
            if (target.closest(this.reactionBtnSelector)) {
                const btn = target.closest(this.reactionBtnSelector) as HTMLElement;
                const reactionType = btn.dataset.reactionType;
                const container = btn.closest(this.reactionContainerSelector) as HTMLElement;
                const postId = container?.dataset.postId;

                if (postId && reactionType) {
                    const currentReaction = this.getCurrentReaction(postId);
                    this.submitReaction(postId, reactionType, reactionType === currentReaction);
                }

                // Hide all popovers
                this.hideAllPopovers();
            }

            // Global click — close if outside the container
            if (!target.closest(this.reactionContainerSelector)) {
                this.hideAllPopovers();
            }
        });
    }


    togglePopover() {
        if (!this.$popover) return;

        const popoverInstance = Popover.getInstance(this.$popover);

        // Hide all others first
        this.hideAllPopovers();

        if (popoverInstance) {
            // If already shown, hide it
            popoverInstance.hide();
            this.$popover.setAttribute('aria-expanded', 'false');
        } else {
            // Create new instance and show it
            const newPopover = new Popover(this.$popover, {
                trigger: 'manual',
                placement: 'top',
                html: true,
                content: () => {
                    const contentElement = this.$popover?.closest(this.reactionContainerSelector)?.querySelector(this.reactionPopoverContentSelector);
                    const cloned = contentElement?.cloneNode(true) as HTMLElement;
                    if (cloned) cloned.style.display = 'block';
                    return cloned || "Something went wrong!";
                }
            });
            newPopover.show();
            this.$popover.setAttribute('aria-expanded', 'true');
        }
    }

    hideAllPopovers() {
        const triggers = document.querySelectorAll(this.popoverTriggerSelector);
        triggers.forEach(trigger => {
            const instance = Popover.getInstance(trigger);
            if (instance) instance.hide();
            trigger.setAttribute('aria-expanded', 'false');
        });
    }

    initializePopovers() {
        const triggers = document.querySelectorAll(this.popoverTriggerSelector);

        triggers.forEach(triggerEl => {
            const contentEl = triggerEl
                .closest(this.reactionPopoverContentSelector)
                ?.querySelector(this.reactionPopoverContentSelector);

            new Popover(triggerEl, {
                trigger: 'click',
                placement: 'top',
                html: true,
                content: () => {
                    const cloned = contentEl?.cloneNode(true) as HTMLElement;
                    if (cloned) cloned.style.display = 'block';
                    return cloned || "Something went wrong!";
                }
            });
        });

        // $(this.popoverTriggerSelector).popover({
        //     trigger: 'click',
        //     placement: 'top',
        //     html: true,
        //     content: () => {
        //         const reactionPopoverContent = this.$popover?.siblings(this.reactionPopoverContentSelector);
        //         return reactionPopoverContent?.clone().show() ?? "Something went wrong!";
        //     }
        // });
    }

    getCurrentReaction(postId: string) {
        const $reactionContainer = $(`${this.reactionContainerSelector}[data-post-id="${postId}"]`);
        return $reactionContainer.data('active-reaction');
    }

    submitReaction(postId: string, reactionType: string, isDeleting: boolean) {
        const url = isDeleting ? `/Reactions/Delete` : `/Reactions/Create`;
        const method = isDeleting ? 'DELETE' : 'POST';

        const postIdInt = parseInt(postId);
        const reactionTypeInt = parseInt(reactionType);

        if (!(postIdInt && reactionTypeInt)) throw("Post Id or Reaction Id is missing");

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify({ PostId: postIdInt, Type: reactionTypeInt }),
            success: () => {
                this.updateReactions(postId);
            },
            error: () => {
                alert('Error processing reaction. Please try again.');
            }
        });
    }

    updateReactions(postId: string) {
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

export default Reactions;
export { Reactions };