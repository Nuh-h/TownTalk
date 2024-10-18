using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TownTalkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TownTalkDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Apply any pending migrations
    context.Database.Migrate();

    // Seed the database with initial data
    TownTalkDbContext.SeedData(context, userManager);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Handle errors for non-dev environment
    app.UseHsts(); // Use HTTP Strict Transport Security in production
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Serve static files (images, CSS, JS, etc.)
app.UseStaticFiles(); // Serves static files like CSS/JS/images

// Route requests
app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication(); // This was missing
app.UseAuthorization();  // Authorization middleware

// Map default route for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Run the application
app.Run();
