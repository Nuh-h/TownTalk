namespace TownTalk.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Moq;
using TownTalk.Web.Hubs;
using TownTalk.Web.Models;
using TownTalk.Web.Repositories.Interfaces;
using TownTalk.Web.Services;

[TestFixture]
public class NotificationServiceTests
{
    private Mock<INotificationRepository> _mockNotificationRepository;
    private Mock<IHubContext<NotificationHub>> _mockHubContext;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private NotificationService _notificationService;
    private Mock<IClientProxy> _mockClientProxy;

    [SetUp]
    public void Setup()
    {
        // Setup UserManager mock
        Mock<IUserStore<ApplicationUser>>? userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        // Setup SignalR mocks differently
        _mockClientProxy = new Mock<IClientProxy>();
        _mockHubContext = new Mock<IHubContext<NotificationHub>>();

        Mock<IHubClients>? mockClients = new Mock<IHubClients>();
        mockClients.Setup(c => c.User(It.IsAny<string>()))
                  .Returns(_mockClientProxy.Object);

        _mockHubContext.Setup(x => x.Clients)
                      .Returns(mockClients.Object);

        _mockNotificationRepository = new Mock<INotificationRepository>();

        _notificationService = new NotificationService(
            notificationRepository: _mockNotificationRepository.Object,
            hubContext: _mockHubContext.Object,
            userManager: _mockUserManager.Object
        );
    }

    [Test]
    public async Task NotifyUserAsync_WhenNotificationDoesNotExist_CreatesAndSendsNotification()
    {
        // Arrange
        string userId = "user1";
        string message = "Test message";
        int postId = 1;
        string senderId = "sender1";
        string type = "Test";

        var sender = new ApplicationUser { Id = senderId, DisplayName = "Test User" };

        _mockUserManager.Setup(x => x.FindByIdAsync(senderId))
            .ReturnsAsync(sender);

        _mockNotificationRepository.Setup(x => x.NotificationExistsAsync(userId, postId, senderId, type))
            .ReturnsAsync(false);

        // Setup SignalR mock to track method calls
        bool signalRCalled = false;
        _mockClientProxy.Setup(x => x.SendCoreAsync(
            It.IsAny<string>(),
            It.IsAny<object[]>(),
            default))
            .Callback<string, object[], CancellationToken>((method, args, token) =>
            {
                signalRCalled = true;
                var notification = args[0] as Notification;

                Assert.Multiple(() =>
                {
                    Assert.That(method, Is.EqualTo("ReceiveNotification"));
                    Assert.That(notification?.UserId, Is.EqualTo(userId));
                    Assert.That(notification?.Message, Is.EqualTo(message));
                });
            })
            .Returns(Task.CompletedTask);

        // Act
        await _notificationService.NotifyUserAsync(userId, message, postId, senderId, type);

        // Assert
        _mockNotificationRepository.Verify(
            x => x.AddNotificationAsync(It.Is<Notification>(n =>
                n.UserId == userId &&
                n.Message == message &&
                n.PostId == postId &&
                n.SenderId == senderId &&
                n.Type == type &&
                !n.IsRead
            )),
            Times.Once
        );

        Assert.That(signalRCalled, Is.True, "SignalR notification was not sent");
    }
    [Test]
    public async Task NotifyUserAsync_WhenNotificationExists_DoesNotCreateDuplicate()
    {
        // Arrange
        string userId = "user1";
        string senderId = "sender1";
        var sender = new ApplicationUser { Id = senderId, DisplayName = "Test User" };

        _mockUserManager.Setup(x => x.FindByIdAsync(senderId))
            .ReturnsAsync(sender);

        _mockNotificationRepository.Setup(x => x.NotificationExistsAsync(
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _notificationService.NotifyUserAsync(userId, "message", 1, senderId, "Test");

        // Assert
        _mockNotificationRepository.Verify(
            x => x.AddNotificationAsync(It.IsAny<Notification>()),
            Times.Never
        );
    }

    [Test]
    public async Task NotifyFollowAsync_WhenDifferentUsers_SendsNotification()
    {
        // Arrange
        string followerId = "follower1";
        string followedId = "followed1";
        var follower = new ApplicationUser { Id = followerId, DisplayName = "Follower User" };

        _mockUserManager.Setup(x => x.FindByIdAsync(followerId))
            .ReturnsAsync(follower);

        // Act
        await _notificationService.NotifyFollowAsync(followerId, followedId);

        // Assert
        _mockNotificationRepository.Verify(
            x => x.AddNotificationAsync(It.Is<Notification>(n =>
                n.UserId == followedId &&
                n.SenderId == followerId &&
                n.Type == "Follow"
            )),
            Times.Once
        );
    }

    [Test]
    public async Task NotifyFollowAsync_WhenSameUser_DoesNotSendNotification()
    {
        // Arrange
        string userId = "user1";

        // Act
        await _notificationService.NotifyFollowAsync(userId, userId);

        // Assert
        _mockNotificationRepository.Verify(
            x => x.AddNotificationAsync(It.IsAny<Notification>()),
            Times.Never
        );
    }
}

//[TestFixture]
//public class NotificationRepositoryTests
//{
//    private DbContextOptions<TownTalkDbContext> _options;
//    private TownTalkDbContext _context;
//    private NotificationRepository _repository;

//    [SetUp]
//    public void Setup()
//    {
//        _options = new DbContextOptionsBuilder<TownTalkDbContext>()
//            .UseInMemoryDatabase(databaseName: "TownTalkTest_" + Guid.NewGuid().ToString())
//            .Options;

//        _context = new TownTalkDbContext(_options);
//        _repository = new NotificationRepository(_context);
//    }

//    [TearDown]
//    public void TearDown()
//    {
//        _context.Database.EnsureDeleted();
//        _context.Dispose();
//    }

//    [Test]
//    public async Task AddNotificationAsync_AddsNotificationToDatabase()
//    {
//        // Arrange
//        var notification = new Notification
//        {
//            UserId = "user1",
//            Message = "Test notification",
//            PostId = 1,
//            SenderId = "sender1",
//            Type = "Test",
//            CreatedAt = DateTime.UtcNow,
//            IsRead = false
//        };

//        // Act
//        await _repository.AddNotificationAsync(notification);

//        // Assert
//        var savedNotification = await _context.Notifications.FirstOrDefaultAsync();
//        Assert.That(savedNotification, Is.Not.Null);
//        Assert.That(savedNotification.Message, Is.EqualTo("Test notification"));
//    }

//    [Test]
//    public async Task GetUserNotificationsAsync_ReturnsUserNotifications()
//    {
//        // Arrange
//        var userId = "user1";
//        var notifications = new List<Notification>
//        {
//            new Notification { UserId = userId, Message = "Test 1", CreatedAt = DateTime.UtcNow },
//            new Notification { UserId = userId, Message = "Test 2", CreatedAt = DateTime.UtcNow },
//            new Notification { UserId = "other", Message = "Other", CreatedAt = DateTime.UtcNow }
//        };

//        await _context.Notifications.AddRangeAsync(notifications);
//        await _context.SaveChangesAsync();

//        // Act
//        var result = await _repository.GetUserNotificationsAsync(userId);

//        // Assert
//        Assert.That(result.Count(), Is.EqualTo(2));
//        Assert.That(result.All(n => n.UserId == userId), Is.True);
//    }

//    [Test]
//    public async Task MarkAsReadAsync_MarksNotificationAsRead()
//    {
//        // Arrange
//        var notification = new Notification
//        {
//            UserId = "user1",
//            Message = "Test",
//            IsRead = false
//        };
//        await _context.Notifications.AddAsync(notification);
//        await _context.SaveChangesAsync();

//        // Act
//        await _repository.MarkAsReadAsync(notification.Id);

//        // Assert
//        var updated = await _context.Notifications.FindAsync(notification.Id);
//        Assert.That(updated.IsRead, Is.True);
//    }

//    [Test]
//    public async Task NotificationExistsAsync_ReturnsCorrectResult()
//    {
//        // Arrange
//        var notification = new Notification
//        {
//            UserId = "user1",
//            PostId = 1,
//            SenderId = "sender1",
//            Type = "Test",
//            IsRead = false
//        };
//        await _context.Notifications.AddAsync(notification);
//        await _context.SaveChangesAsync();

//        // Act
//        var exists = await _repository.NotificationExistsAsync("user1", 1, "sender1", "Test");
//        var doesntExist = await _repository.NotificationExistsAsync("user2", 1, "sender1", "Test");

//        // Assert
//        Assert.That(exists, Is.True);
//        Assert.That(doesntExist, Is.False);
//    }
//}
