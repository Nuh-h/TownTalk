import $ from 'jquery';
import { CommentResponse } from './interface';

export class CommentHandler {

  constructor() {
    $(document).on('click', '[data-toggle="js-comments-wrapper"]', function () {
      var target = $(this).data('target');
      $(target).toggleClass('show');
      $(target).toggleClass('collapse');
    });

    this.initializeCommentHandlers();

  }

  /**
   * Handle form submission
   */
  private handleCommentFormSubmission(): void {

    $(document).on('submit', '.comment-form', (e: JQuery.TriggeredEvent) => {

      e.preventDefault();
      const form = $(e.currentTarget);

      $.ajax({
        type: 'POST',
        url: form.attr('action')!,
        data: form.serialize(),
        success: (data: CommentResponse) => {
          if (data.success) {
            this.updateCommentList(data);

            form[0].reset();
          } else {
            alert('Error creating comment. Please try again.');
          }
        },
        error: () => {
          alert('There was an error submitting your comment. Please try again.');
        }
      });
    });
  }

  /**
   * Toggles reply form visibility
   */
  private handleReplyFormToggle(): void {

    $(document).on('click', '.reply-button', (e: JQuery.TriggeredEvent) => {

      const replyFormId = $(e.currentTarget).data('reply-form-id');
      $(`#${replyFormId}`).toggle();

    });
  }

  /**
   * Delete a comment
   */
  private handleCommentDeletion(): void {

    $(document).on('click', '.delete-comment', (e: JQuery.TriggeredEvent) => {

      const commentId = $(e.currentTarget).data('comment-id');

      $.ajax({
        type: 'POST',
        url: `/Comments/Delete/${commentId}`,
        data: { id: commentId },
        success: (response: { success: boolean; id: number }) => {
          if (response.success) {
            $(`[data-comment-id="${response.id}"]`).remove();
          } else {
            alert('Error deleting the comment. Please try again.');
          }
        },
        error: () => {
          alert('There was an error deleting the comment. Please try again.');
        }
      });

    });
  }

  /**
   * Handle URL fragment for comments
   */
  private handleCommentLink(): void {

    const hash = window.location.hash;

    if (hash.startsWith('#comment-')) {

      const commentId = hash.replace('#comment-', '');
      const commentElement = $(`.comment-item[data-comment-id='${commentId}']`);

      if (commentElement.length) {
        const postId = commentElement.closest('.comment-list')?.attr('id')?.replace('commentList-', '');

        if (postId) {
          // $(`#comments-${postId}`).collapse('show');
          $('html, body').animate({
            scrollTop: commentElement.offset()?.top
          }, 500);
        }
      }
    }
  }

  // Function to initialize all comment-related event handlers
  private initializeCommentHandlers(): void {



    this.handleCommentFormSubmission();
    this.handleReplyFormToggle();
    this.handleCommentDeletion();

    this.handleCommentLink();

    $(window).on('hashchange', () => this.handleCommentLink());
  }

  private updateCommentList(data: CommentResponse) {

    const newCommentHtml = `
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
    </div>`;

    const repliesContainer = $(`[data-comment-id="${data.parentCommentId}"] .replies-list`);
    repliesContainer.prepend(newCommentHtml);
  }
}
