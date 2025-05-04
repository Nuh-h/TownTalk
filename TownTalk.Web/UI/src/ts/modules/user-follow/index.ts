import { FollowResponse } from "./interface";

export default class UserFollow {
    private profileId: string;
    private followButton: HTMLElement | null;

    constructor(profileId: string) {
        this.profileId = profileId;
        this.followButton = document.querySelector('.js-profile-btn');
        this.followButton?.addEventListener('click', () => this.toggleFollow());
    }

    private async toggleFollow(): Promise<void> {
        try {
            const response = await fetch('/Profile/ToggleFollow', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: new URLSearchParams({ userId: this.profileId }),
            });

            if (!response.ok) {
                console.error('Failed to toggle follow status.');
                return;
            }

            const data: FollowResponse = await response.json();
            this.updateFollowButton(data);
            this.updateProfileStats(data);
        } catch (error) {
            console.error('An error occurred while toggling follow:', error);
        }
    }

    private updateFollowButton(response: FollowResponse): void {
        if (!this.followButton) return;

        if (response.isFollowing) {
            this.followButton.textContent = 'Unfollow';
            this.followButton.classList.remove('btn-primary');
            this.followButton.classList.add('btn-danger');
        } else {
            this.followButton.textContent = 'Follow';
            this.followButton.classList.remove('btn-danger');
            this.followButton.classList.add('btn-primary');
        }
    }

    private updateProfileStats(response: FollowResponse): void {
        const followersEl = document.getElementById('followers-count');
        const followingEl = document.getElementById('following-count');
        const mutualEl = document.getElementById('mutual-followers-count');

        if (followersEl) followersEl.textContent = response.followersCount.toString();
        if (followingEl) followingEl.textContent = response.followingCount.toString();
        if (mutualEl) mutualEl.textContent = `You have ${response.mutualFollowersCount} mutual followers`;
    }
}
