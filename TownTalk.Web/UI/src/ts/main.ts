import $ from 'jquery';
import "jquery-ui";
import 'bootstrap';

import { CommentHandler } from './modules/comments';
import { Notifications } from './modules/notifications';
import { Reactions } from './modules/reactions';
import UserCharts from './modules/charts';
import UserFollow from './modules/user-follow';
import { GetVisNetwork } from './modules/vis-connections';

$(document).ready(function () {
    const notificationsHandler = new Notifications();
    const commentHandler = new CommentHandler();
    const reactionsHandler = new Reactions();

    const scrollToTopBtn = $('#scrollToTopBtn');

    $(window).scroll(function () {
        if ($(this).scrollTop()! > 200) {
            scrollToTopBtn.fadeIn();
        } else {
            scrollToTopBtn.fadeOut();
        }
    });

    scrollToTopBtn.on('click', function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
    });


    // Setting up charts

    $('.js-vis-network')?.on('click', () => {
        GetVisNetwork();
    });

    // Profile charts

    const profileEl = document.querySelector<HTMLElement>('.js-profile-user');
    const profileId = profileEl?.dataset.profileId;

    if (profileId) {
        const charts = new UserCharts(profileId);
        charts.initializeCharts();
        new UserFollow(profileId);
    }

    // Admin dashboard charts
    const adminChartsEl = document.querySelector<HTMLElement>('.js-admin-charts');
    const adminProfileId = adminChartsEl?.dataset.profileid;

    if (adminProfileId) {
        const charts = new UserCharts(adminProfileId);
        charts.initializeAdminCharts();
    }

    // Users list

    interface UserData {
        id: string;
        displayName: string;
        profilePictureUrl: string;
        isMutual: boolean;
    }

    if (profileId) {
        loadTabData('followers', profileId);

        const tabs = document.querySelectorAll<HTMLAnchorElement>('#profileTabs a');
        tabs.forEach((tab) => {
            tab.addEventListener('click', (e) => {
                e.preventDefault();

                const tabId = tab.getAttribute('href')?.substring(1);
                if (tabId) {
                    loadTabData(tabId, profileId);
                    setActiveTab(tabId);
                }
            });
        });
    }

    function loadTabData(activeTab: string, userId: string): void {
        const url = `/Profile/GetUsers?userId=${encodeURIComponent(userId)}&tab=${encodeURIComponent(activeTab)}`;

        fetch(url)
            .then((response) => response.json())
            .then((data: UserData[]) => {
                document.querySelectorAll<HTMLElement>('#followersList, #followingList, #othersList').forEach((list) => {
                    list.innerHTML = '';
                });

                const listItems = data.map((item) => {
                    const profileNames = item.displayName.split(' ');
                    const profileInitials = profileNames[0]?.[0] ?? '';
                    const secondInitial = profileNames.length > 1 ? profileNames[1]?.[0] ?? '' : '';
                    const profileName = profileInitials + secondInitial;

                    const buttonClass = activeTab === 'following' || item.isMutual ? 'btn-danger' : 'btn-primary';
                    const buttonText = activeTab === 'following' || item.isMutual ? 'Unfollow' : 'Follow';

                    return `<li class="list-group-item p-0" style="background-color: transparent">
                    <div class="card" style="background-color: #00000057; display: flex; flex-direction: row; align-items: center;">
                        <div class="card-header">
                            <img src="${item.profilePictureUrl}" alt="Profile Picture" class="rounded-circle mb-0" style="width: 85px; height: 85px;">
                        </div>
                        <div class="card-body">
                            <a class="text-white d-block pb-3 px-0" href="/Profile?userId=${item.id}">${item.displayName}</a>
                            <button class="btn ${buttonClass} js-profile-btn" data-profile-id="${item.id}" data-is-following="${activeTab === 'following'}">${buttonText}</button>
                        </div>
                    </div>
                </li>`;
                }).join('');

                const listElement = document.getElementById(`${activeTab}List`);
                if (listElement) {
                    listElement.innerHTML = listItems;
                    document.querySelectorAll<HTMLElement>(`#${activeTab}List .js-profile-btn`).forEach((btn) => {
                        const profileId = btn.getAttribute('data-profile-id');
                        if (profileId) {
                            new UserFollow(profileId);
                        }
                    });
                }
            })
            .catch((error) => {
                console.error('Error loading tab data:', error);
            });
    }

    function setActiveTab(activeTab: string): void {
        document.querySelectorAll<HTMLElement>('#profileTabs .nav-link').forEach((tab) => {
            tab.classList.remove('active');
        });

        document.querySelectorAll<HTMLElement>('.tab-pane').forEach((tabContent) => {
            tabContent.classList.remove('show', 'active');
        });

        const activeTabLink = document.getElementById(`${activeTab}-tab`);
        const activeTabContent = document.getElementById(activeTab);

        activeTabLink?.classList.add('active');
        activeTabContent?.classList.add('show', 'active');
    }



    //Posts pagination

    (window as any)['loadPosts'] = function (page: any) {
        const urlParams = new URLSearchParams(window.location.search);
        urlParams.set("page", page);

        const queryString = urlParams.toString();

        fetch(`/Posts/GetPosts?${queryString}`)
            .then(response => response.text())
            .then(data => {
                // Replace the PostList and Pagination parts with new content
                document.getElementById("resultsContainer")!.innerHTML = data;

                reactionsHandler.init();

                $('html, body').animate({ scrollTop: 0 }, 800);

            });
    }

});