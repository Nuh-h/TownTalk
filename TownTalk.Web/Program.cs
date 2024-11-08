using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TownTalk.Repositories;
using TownTalk.Repositories.Interfaces;
using TownTalk.Services;
using TownTalk.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TownTalkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserFollowService, UserFollowService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TownTalkDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Configure the login, logout, and access denied paths
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Add MVC services (controllers + views)
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    IServiceProvider? services = scope.ServiceProvider;
    TownTalkDbContext? context = services.GetRequiredService<TownTalkDbContext>();
    UserManager<ApplicationUser>? userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Apply any pending migrations
    context.Database.Migrate();

    // Seed the database with initial data
    TownTalkDbContext.SeedData(context, userManager);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Serve static files (images, CSS, JS, etc.)
app.UseStaticFiles();

// Route requests
app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map default route for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapHub<NotificationHub>("/notificationHub");


// Run the application
app.Run();
