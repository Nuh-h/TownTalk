interface PostByMonth {
    month: string;
    count: number;
}

interface FollowersGrowth {
    year: number;
    month: number;
    count: number;
    date: string;
    cumulative?: number;
}

interface UserActivityByMonth {
    month: string;
    postCount: number;
    commentCount: number;
    reactionCount: number;
}

interface FollowTrendsByMonth {
    month: string;
    followersGained: number;
    followingGained: number;
}