using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Hubs;
using TownTalk.Web.Models;
using TownTalk.Web.Repositories;
using TownTalk.Web.Repositories.Interfaces;
using TownTalk.Web.Services;
using TownTalk.Web.Services.Interfaces;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TownTalkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserFollowService, UserFollowService>();
builder.Services.AddScoped<IGraphService, GraphService>();
builder.Services.AddScoped<UserDataSeeder>();


builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<TownTalkDbContext>();


builder.Services.ConfigureApplicationCookie(options =>
{
    // Configure the login, logout, and access denied paths
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Add MVC services (controllers + views)
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger documentation
builder.Services.AddSwaggerGen(); // Add Swagger generation

WebApplication? app = builder.Build();

// Apply migrations and seed data
using (IServiceScope? scope = app.Services.CreateScope())
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger UI
    app.UseSwaggerUI(); // Display Swagger UI at /swagger
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
