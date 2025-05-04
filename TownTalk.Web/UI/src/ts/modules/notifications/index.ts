import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import * as dateFns from 'date-fns';
import * as bootstrap from "bootstrap";
import $ from 'jquery';

class Notifications {
    private displayedNotifications: Set<string>;
    private scrollToId: string | null;
    private connection: HubConnection;
    private notificationBellSelector: string = ".js-notification-bell";
    private viewNotificationButtonSelector: string = ".view-post-button";
    private notificationModalSelector: string = "#notificationModal";

    constructor() {
        this.connection = new HubConnectionBuilder()
            .withUrl("/notificationHub")
            .build();

        this.displayedNotifications = new Set<string>();
        this.scrollToId = null;

        this.init();
    }

    private async init(): Promise<void> {
        await this.startConnection();
        this.setupEventListeners();
    }

    private setupEventListeners(): void {
        this.connection.on("ReceiveNotification", this.handleNewNotification.bind(this));
        this.connection.on("ReceiveAllNotifications", this.handleAllNotifications.bind(this));

        $(this.notificationBellSelector).on("click", () => {
            this.fetchAllNotifications();
            this.toggleNotificationIndicator(false);
        });
    }

    private async handleNewNotification(notification: Notification): Promise<void> {

        if (!this.displayedNotifications.has(notification.id)) {

            this.addNotificationToList(notification);
            this.displayedNotifications.add(notification.id);

            if (!notification.isRead) {
                this.toggleNotificationIndicator(true);
            }

            if (notification.type === "Reaction" && (window as any).updateReactionsForPost) {
                (window as any).updateReactionsForPost(notification.postId);
            }
        }
        this.initViewPostEvents();
    }

    private handleAllNotifications(notifications: Notification[]): void {
        console.log({ notifications });
        notifications.forEach(notification => this.handleNewNotification(notification));
        this.initViewPostEvents();
    }

    private initViewPostEvents(): void {
        $(this.viewNotificationButtonSelector).off("click").on("click", (e) => {
            e.stopPropagation();

            const notificationId = $(e.currentTarget).data("notification-id");
            this.scrollToId = $(e.currentTarget).data("post-id");

            this.closeNotificationModal();

            if ($(e.currentTarget).closest(`li[data-notification-id=${notificationId}]`).hasClass("notification-unread")) {
                this.markNotificationAsRead(notificationId);
            }
        });

        $(this.notificationModalSelector).on('hidden.bs.modal', this.scrollToPost.bind(this));
    }

    private async startConnection(): Promise<void> {
        try {
            await this.connection.start();
        } catch (err) {
            console.error(err);
            setTimeout(() => this.startConnection(), 5000);
        }
    }

    private async fetchAllNotifications(): Promise<void> {
        await this.connection.invoke("FetchAllNotifications");
    }

    private toggleNotificationIndicator(hasNotifications: boolean): void {
        const indicator = document.getElementById("notification-indicator");
        if (indicator) {
            indicator.setAttribute("fill", hasNotifications ? "red" : "transparent");
        }
    }

    private addNotificationToList(notification: Notification): void {

        const timestamp = new Date(notification.createdAt);
        const relativeTime = dateFns.formatDistanceToNow(timestamp, { addSuffix: true });
        const listItem = document.createElement("li");
        listItem.className = `list-group-item d-flex justify-content-between align-items-center text-dark ${notification.isRead ? "" : "notification-unread"}`;
        const profileUrl = `/user/profile/${notification.senderId}`;
        const displayName = `<span class="sender-display-name" style="font-weight:600">${notification.senderDisplayName}</span>`;
        const notificationMessage = `<div>${notification.senderDisplayName != null ? displayName : ""}${notification.message.replace(notification?.senderDisplayName ?? "", "")}</div>`;

        let postButton = '';
        if (notification.type === "Reaction" || notification.type === "Comment") {
            postButton = `<button class="btn btn-link view-post-button" data-post-id="${notification.postId}" data-notification-id="${notification.id}">View Post</button>`;
        }

        listItem.innerHTML = notificationMessage + (postButton ? ` ${postButton}` : '');
        const badge = document.createElement("span");
        badge.className = "badge bg-primary rounded-pill text-white";
        badge.textContent = relativeTime;
        listItem.appendChild(badge);
        listItem.dataset.notificationId = notification.id;
        listItem.dataset.postId = notification.postId;

        if (notification.type === "Reaction") {
            document.getElementById("notificationListReaction")?.appendChild(listItem);
        } else if (notification.type === "Comment") {
            document.getElementById("notificationListComment")?.appendChild(listItem);
        } else if (notification.type === "Follow") {
            document.getElementById("notificationListFollow")?.appendChild(listItem);
        }

        document.getElementById("notificationListAll")?.appendChild(listItem.cloneNode(true));
    }

    private closeNotificationModal(): void {
        const modalElement = document.getElementById('notificationModal');
        if (modalElement) {
            const modalInstance = bootstrap.Modal.getInstance(modalElement);

            modalInstance?.hide();
        }
    }

    private scrollToPost(): void {
        const postElement = document.getElementById(`post-${this.scrollToId}`);
        if (postElement) {

            postElement.style = `
                ${postElement.style};
                background: inherit;
                mix-blend-mode: plus-lighter;
                border: 1px solid;
                border-radius: 12px;
            `;
            postElement.scrollIntoView({ behavior: 'smooth' });

            setTimeout(() => {
                postElement.style = `background-color:rgba(255, 255, 255, 0.05)`;
                this.scrollToId = null;
            }, 3000);
        }
    }

    private markNotificationAsRead(notificationId: string): void {
        this.connection.invoke("MarkNotificationAsRead", parseInt(notificationId))
            .then(response => {
                console.log("Notification marked as read successfully:", response);
                this.toggleNotificationIndicator(false);
            }).catch(err => {
                console.error(err);
            });
    }
}

export { Notifications };
export default Notifications;