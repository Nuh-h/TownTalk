interface Notification {
    id: string;
    senderId: string;
    senderDisplayName: string | null;
    message: string;
    type: 'Reaction' | 'Comment' | 'Follow';
    isRead: boolean;
    createdAt: string;
    postId: string;
}