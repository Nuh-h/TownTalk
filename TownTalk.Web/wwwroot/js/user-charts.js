class UserCharts {
    constructor(userId) {
        this.userId = userId;
        this.apiUrl = '/api/usercharts';
    }

    async getPostsByMonth() {
        const response = await fetch(`${this.apiUrl}/postsbymonth/${this.userId}`);
        if (!response.ok) {
            console.error('Error fetching posts data');
            return [];
        }
        return await response.json(); // Returns data in { month: string, count: int }
    }

    async getFollowersGrowth() {
        const response = await fetch(`${this.apiUrl}/followersgrowth/${this.userId}`);
        if (!response.ok) {
            console.error('Error fetching followers data');
            return [];
        }
        return await response.json(); // Returns data in { year: number, month: number, count: int }
    }

    //TODO: adapt to new data type returned ^
    // Initialize and render the Posts by Month Chart
    async renderPostsByMonthChart() {
        const data = await this.getPostsByMonth();
        const ctx = document.getElementById('postsByMonthChart').getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.map(item => item.date.length === 7 ? item.date : `0${item.date}`),
                datasets: [{
                    label: 'Posts Created',
                    data: data.sort((a, b) => new Date(`01/` + a.date) - new Date(`01/` + b.date))
                        .map((item, index) => {
                            console.log(index);
                            if (index > 0) { data[index].cumulative = data[index - 1].cumulative + item.count; }
                            else { data[index].cumulative = item.count; }

                            return data[index].cumulative;
                        }),
                    backgroundColor: '#4C8BF5',
                    borderColor: '#1f1f1f',
                    borderWidth: 1
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
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)',
                        },
                        ticks: {
                            color: 'rgba(255, 255, 255, 0.7)'
                        }
                    },
                    y: {
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)',
                        },
                        ticks: {
                            color: 'rgba(255, 255, 255, 0.7)'
                        }
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
                },
                responsive: true
            }
        });
    }

    // Initialize and render the Followers Growth Chart
    async renderFollowersGrowthChart() {
        const data = await this.getFollowersGrowth();
        const ctx = document.getElementById('followersGrowthChart').getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.map(item => item.date),
                datasets: [{
                    label: 'Followers Growth',
                    data: data.sort((a, b) => new Date(`01/` + a.date) - new Date(`01/` + b.date))
                        .map((item, index) => {
                            if (index > 0) { data[index].cumulative = data[index - 1].cumulative + item.count; }
                            else { data[index].cumulative = item.count; }

                            return data[index].cumulative;
                        }),
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
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)',
                        },
                        ticks: {
                            color: 'rgba(255, 255, 255, 0.7)'
                        }
                    },
                    y: {
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)',
                        },
                        ticks: {
                            color: 'rgba(255, 255, 255, 0.7)'
                        }
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
                },
                responsive: true
            }
        });
    }

    async initializeCharts() {
        await Promise.all([this.renderPostsByMonthChart(), this.renderFollowersGrowthChart()]);
    }
}

$(document).ready(() => {
    const profileId = $('.js-profile-user').data("profile-id");
    new UserCharts(profileId).initializeCharts();
});