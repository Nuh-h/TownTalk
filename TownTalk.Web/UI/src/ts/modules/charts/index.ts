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

    public async renderPostsByMonthChart(): Promise<void> {
        const data = await this.getPostsByMonth();
        const ctx = (document.getElementById('postsByMonthChart') as HTMLCanvasElement)?.getContext('2d');
        if (!ctx) return;

        new Chart(ctx, {
            type: 'bubble',
            data: {
                datasets: [{
                    label: 'Posts Created',
                    data: data.map(item => ({
                        x: parseInt(item.year.toString()),
                        y: parseInt(item.month),
                        r: item.count * 2
                    })),
                    backgroundColor: '#4C8BF5',
                    borderColor: '#1f1f1f',
                }]
            },
            options: {}
        });
    }

    public async renderFollowersGrowthChart(): Promise<void> {
        const data = await this.getFollowersGrowth();
        const ctx = (document.getElementById('followersGrowthChart') as HTMLCanvasElement)?.getContext('2d');
        if (!ctx) return;

        const sortedData = data.sort((a, b) => new Date(`01/${a.date}`).getTime() - new Date(`01/${b.date}`).getTime());

        sortedData.forEach((item, index) => {
            item.cumulative = index === 0 ? item.count : sortedData[index - 1].cumulative! + item.count;
        });

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: sortedData.map(item => item.date),
                datasets: [{
                    label: 'Followers Growth',
                    data: sortedData.map(item => item.cumulative!),
                    borderColor: 'rgba(75, 192, 192, 1)',
                    fill: false
                }]
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

    public async initializeCharts(): Promise<void> {
        await Promise.all([
            this.renderPostsByMonthChart(),
            this.renderFollowersGrowthChart()
        ]);
    }
}

export default UserCharts;
export { UserCharts };
