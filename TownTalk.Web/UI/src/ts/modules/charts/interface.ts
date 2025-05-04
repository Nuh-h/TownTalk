interface PostByMonth {
    month: string;
    count: number;
    year: number;
}

interface FollowersGrowth {
    year: number;
    month: number;
    count: number;
    date: string;
    cumulative?: number;
}