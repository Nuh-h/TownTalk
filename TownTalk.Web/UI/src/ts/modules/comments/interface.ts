export interface CommentResponse {
    success: boolean;
    userDisplayName: string;
    createdAt: string;
    content: string;
    id: number;
    postId: number;
    parentCommentId: number;
}
