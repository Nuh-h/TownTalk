// <copyright file="Program.cs" company="TownTalk">
// Copyright (c) Town.Talk. All rights reserved.
// </copyright>

#pragma warning disable SA1124 // DoNotUseRegions

#region namespaces
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TownTalk.Web.Data;
using TownTalk.Web.Hubs;
using TownTalk.Web.Models;
using TownTalk.Web.Repositories;
using TownTalk.Web.Repositories.Interfaces;
using TownTalk.Web.Services;
using TownTalk.Web.Services.Interfaces;

#endregion

#region Services

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

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
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// MVC services (controllers + views) and API documentation
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication? app = builder.Build();

// Apply migrations and seed data
using (IServiceScope? scope = app.Services.CreateScope())
{
    IServiceProvider? services = scope.ServiceProvider;
    TownTalkDbContext? context = services.GetRequiredService<TownTalkDbContext>();
    UserManager<ApplicationUser>? userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    context.Database.Migrate();

    TownTalkDbContext.SeedData(context, userManager);
}

#endregion

#region Middleware(s)

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

#endregion

#region Routing

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapHub<NotificationHub>("/notificationHub");

#endregion

app.Run();
