import Chart from 'chart.js/auto';

/**
 * Represents a set of chart utilities for visualizing user-related data such as posts by month and followers growth.
 *
 * @remarks
 * This class fetches user statistics from the backend API and renders charts using Chart.js.
 * It provides methods to render a bubble chart for posts created by month and a line chart for followers growth over time.
 *
 * @example
 * ```typescript
 * const charts = new UserCharts('user123');
 * charts.initializeCharts();
 * ```
 *
 * @public
 */
class UserCharts {
    private userId: string;
    private apiUrl: string;

    constructor(userId: string) {
        this.userId = userId;
        this.apiUrl = '/api/users';
    }

    private async getPostsByMonth(): Promise<PostByMonth[]> {
        const response = await fetch(`${this.apiUrl}/postsbymonth/${this.userId}`);
        if (!response.ok) {
            console.error('Error fetching posts data');
            return [];
        }
        return await response.json();
    }

    private async getFollowersGrowth(): Promise<FollowersGrowth[]> {
        const response = await fetch(`${this.apiUrl}/followersgrowth/${this.userId}`);
        if (!response.ok) {
            console.error('Error fetching followers data');
            return [];
        }
        return await response.json();
    }

    private async getUserActivityByMonth(): Promise<UserActivityByMonth[]> {
        const response = await fetch(`${this.apiUrl}/activitiesbymonth/${this.userId}`);
        if (!response.ok) {
            console.error('Error fetching activity data');
            return [];
        }
        return await response.json();
    }

    public async renderFollowTrendsChart(): Promise<void> {
        const response = await fetch(`${this.apiUrl}/followtrendsbymonth/${this.userId}`);
        if (!response.ok) {
            console.error('Error fetching follow trends data');
            return;
        }
        const data: FollowTrendsByMonth[] = await response.json();

        const ctx = (document.getElementById('followersGrowthChart') as HTMLCanvasElement)?.getContext('2d');
        if (!ctx) return;

        const labels = data.map(item => item.month);
        const followers = data.map(item => item.followersGained);
        const following = data.map(item => item.followingGained);

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Followers Gained',
                        data: followers,
                        borderColor: 'rgba(75, 192, 192, 1)',
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        fill: false,
                        tension: 0.3
                    },
                    {
                        label: 'Following Gained',
                        data: following,
                        borderColor: 'rgba(241, 196, 15, 1)',
                        backgroundColor: 'rgba(241, 196, 15, 0.2)',
                        fill: false,
                        tension: 0.3
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        labels: {
                            color: 'rgba(255, 255, 255, 0.7)'
                        }
                    },
                    tooltip: {
                        backgroundColor: '#121212',
                        titleColor: '#ffffff',
                        bodyColor: '#ffffff',
                        footerColor: '#ffffff'
                    }
                },
                scales: {
                    x: {
                        grid: { color: 'rgba(255, 255, 255, 0.1)' },
                        ticks: { color: 'rgba(255, 255, 255, 0.7)' }
                    },
                    y: {
                        grid: { color: 'rgba(255, 255, 255, 0.1)' },
                        ticks: { color: 'rgba(255, 255, 255, 0.7)' }
                    }
                },
                elements: {
                    point: {
                        radius: 5,
                        backgroundColor: '#F1C40F',
                        borderWidth: 2
                    }
                },
                layout: {
                    padding: 20
                }
            }
        });
    }

    public async renderUserActivityByMonthChart(): Promise<void> {
        const data = await this.getUserActivityByMonth();
        const ctx = (document.getElementById('userActivityByMonthChart') as HTMLCanvasElement)?.getContext('2d');
        if (!ctx) return;

        const labels = data.map(item => item.month);
        const posts = data.map(item => item.postCount);
        const comments = data.map(item => item.commentCount);
        const reactions = data.map(item => item.reactionCount);

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Posts',
                        data: posts,
                        borderColor: '#4C8BF5',
                        backgroundColor: 'rgba(76, 139, 245, 0.2)',
                        fill: false,
                        tension: 0.3
                    },
                    {
                        label: 'Comments',
                        data: comments,
                        borderColor: '#F1C40F',
                        backgroundColor: 'rgba(241, 196, 15, 0.2)',
                        fill: false,
                        tension: 0.3
                    },
                    {
                        label: 'Reactions',
                        data: reactions,
                        borderColor: '#E74C3C',
                        backgroundColor: 'rgba(231, 76, 60, 0.2)',
                        fill: false,
                        tension: 0.3
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        labels: {
                            color: 'rgba(255, 255, 255, 0.7)'
                        }
                    }
                },
                scales: {
                    x: {
                        grid: { color: 'rgba(255, 255, 255, 0.1)' },
                        ticks: { color: 'rgba(255, 255, 255, 0.7)' }
                    },
                    y: {
                        grid: { color: 'rgba(255, 255, 255, 0.1)' },
                        ticks: { color: 'rgba(255, 255, 255, 0.7)' }
                    }
                }
            }
        });
    }

    public async renderTopStatsBarChart(): Promise<void> {
        const stats = (window as any).topStatsData as {
            totalUsers: number;
            totalPosts: number;
            totalComments: number;
            totalReactions: number;
            newUsersThisMonth: number;
            newPostsThisMonth: number;
            newCommentsThisMonth: number;
            newReactionsThisMonth?: number; // Optional
        };

        console.log('Top Stats Data:', stats);

        if (!stats) {
            console.error('No topStatsData found on window');
            return;
        }

        const labels = ['Users', 'Posts', 'Comments', 'Reactions'];
        const totalData = [
            stats.totalUsers,
            stats.totalPosts,
            stats.totalComments,
            stats.totalReactions
        ];
        const thisMonthData = [
            stats.newUsersThisMonth,
            stats.newPostsThisMonth,
            stats.newCommentsThisMonth,
            stats.newReactionsThisMonth ?? 0
        ];

        const ctx = (document.getElementById('topStatsBarChart') as HTMLCanvasElement)?.getContext('2d');
        if (!ctx) return;

        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Total',
                        data: totalData,
                        backgroundColor: 'rgba(54, 162, 235, 0.7)'
                    },
                    {
                        label: 'This Month',
                        data: thisMonthData,
                        backgroundColor: 'rgba(255, 206, 86, 0.7)'
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        labels: {
                            color: 'rgba(255, 255, 255, 0.7)'
                        }
                    },
                    tooltip: {
                        backgroundColor: '#121212',
                        titleColor: '#ffffff',
                        bodyColor: '#ffffff',
                        footerColor: '#ffffff'
                    }
                },
                scales: {
                    x: {
                        grid: { color: 'rgba(255, 255, 255, 0.1)' },
                        ticks: { color: 'rgba(255, 255, 255, 0.7)' }
                    },
                    y: {
                        grid: { color: 'rgba(255, 255, 255, 0.1)' },
                        ticks: { color: 'rgba(255, 255, 255, 0.7)' }
                    }
                }
            }
        });
    }

    public async renderTopStatsPieCharts(): Promise<void> {
        const stats = (window as any).topStatsData as {
            totalUsers: number;
            totalPosts: number;
            totalComments: number;
            totalReactions: number;
            newUsersThisMonth: number;
            newPostsThisMonth: number;
            newCommentsThisMonth: number;
            newReactionsThisMonth?: number;
        };

        if (!stats) {
            console.error('No topStatsData found on window');
            return;
        }

        // Pie 1: Posts vs Comments vs Reactions (engagement breakdown)
        const engagementCtx = (document.getElementById('engagementPieChart') as HTMLCanvasElement)?.getContext('2d');
        if (engagementCtx) {
            new Chart(engagementCtx, {
                type: 'pie',
                data: {
                    labels: ['Posts', 'Comments', 'Reactions'],
                    datasets: [{
                        data: [stats.totalPosts, stats.totalComments, stats.totalReactions],
                        backgroundColor: [
                            'rgba(54, 162, 235, 0.7)',
                            'rgba(255, 206, 86, 0.7)',
                            'rgba(231, 76, 60, 0.7)'
                        ]
                    }]
                },
                options: {
                    plugins: {
                        legend: {
                            labels: {
                                color: 'rgba(255,255,255,0.9)'
                            }
                        }
                    }
                }
            });
        }

        // Pie 2: New users this month vs Existing users
        const usersCtx = (document.getElementById('usersPieChart') as HTMLCanvasElement)?.getContext('2d');
        if (usersCtx) {
            new Chart(usersCtx, {
                type: 'pie',
                data: {
                    labels: ['New Users This Month', 'Existing Users'],
                    datasets: [{
                        data: [stats.newUsersThisMonth, stats.totalUsers - stats.newUsersThisMonth],
                        backgroundColor: [
                            'rgba(76, 175, 80, 0.7)',
                            'rgba(54, 162, 235, 0.7)'
                        ]
                    }]
                },
                options: {
                    plugins: {
                        legend: {
                            labels: {
                                color: 'rgba(255,255,255,0.9)'
                            }
                        }
                    }
                }
            });
        }
    }

    /**
     * Renders a pie chart for an individual user's stats.
     * Expects a <canvas id="user-stats-chart"></canvas> and a data-user-stats attribute with JSON stats.
     */
    public renderUserStatsPieChart() {
        const statsEl = document.querySelector('[data-user-stats]');
        if (!statsEl) return;

        const userStats = JSON.parse(statsEl.getAttribute('data-user-stats')!);
        const ctx = (document.getElementById('user-stats-chart') as HTMLCanvasElement)?.getContext('2d');
        if (!ctx) return;

        new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['Posts', 'Comments', 'Reactions'],
                datasets: [{
                    data: [userStats.posts, userStats.comments, userStats.reactionsReceived],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.7)',
                        'rgba(255, 206, 86, 0.7)',
                        'rgba(231, 76, 60, 0.7)'
                    ]
                }]
            },
            options: {
                plugins: {
                    legend: {
                        labels: {
                            color: '#fff'
                        }
                    }
                }
            }
        });
    }

    public async initializeCharts(): Promise<void> {
        await Promise.all([
            this.renderUserActivityByMonthChart(),
            this.renderFollowTrendsChart(),
        ]);
    }

    public async initializeAdminCharts(): Promise<void> {
        await Promise.all([
            this.renderTopStatsBarChart(),
            this.renderTopStatsPieCharts(),
            this.renderUserStatsPieChart()
        ]);
    }
}

export default UserCharts;
export { UserCharts };
