class UserFollow {
    constructor(profileId) {
        this.profileId = profileId;
        this.followButton = $(`.js-profile-btn`);
        this.followButton.on('click', this.toggleFollow.bind(this));
    }

    async toggleFollow() {
        const response = await $.ajax({
            url: '/Profile/ToggleFollow',
            method: 'POST',
            data: { userId: this.profileId },
            dataType: 'json',
        });

        this.updateFollowButton(response);
        this.updateProfileStats(response);
    }

    updateFollowButton(response) {
        if (response.isFollowing) {
            this.followButton.text('Unfollow').removeClass('btn-primary').addClass('btn-danger');
        } else {
            this.followButton.text('Follow').removeClass('btn-danger').addClass('btn-primary');
        }
    }

    updateProfileStats(response) {
        $(`#followers-count`).text(response.followersCount);
        $(`#following-count`).text(response.followingCount);
        $(`#mutual-followers-count`).text(`You have ${response.mutualFollowersCount} mutual followers`);
    }
}

$(document).ready(() => {
    const profileId = $('.js-profile-btn').data("profile-id");
    new UserFollow(profileId);
});