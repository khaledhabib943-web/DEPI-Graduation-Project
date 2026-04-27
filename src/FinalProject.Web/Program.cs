using FinalProject.Application;
using FinalProject.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using FinalProject.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinalProject.Web.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<FinalProjectWebContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<FinalProjectWebUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<FinalProject.Web.Data.FinalProjectWebContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// ==============================================================================
// Authentication Configuration (Cookies + Google External Login)
// ==============================================================================
builder.Services.AddAuthentication(options =>
{
    // Default scheme for maintaining the user session
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // Default scheme for external login challenges (Google)
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // Hardened Security for Auth Cookie
    options.Cookie.Name = "SalahlyAuth";
    options.Cookie.HttpOnly = true;               // Prevent XSS access to cookie
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Uses HTTPS in production
    options.Cookie.SameSite = SameSiteMode.Strict; // Prevent CSRF via cross-site requests

    options.ExpireTimeSpan = TimeSpan.FromHours(8); // Session expires after 8 hours
    options.SlidingExpiration = true;              // Renew session on user activity
})
.AddGoogle(options =>
{
    // Google API Credentials from appsettings.json
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});
// ==============================================================================

// Register Infrastructure layer (DbContext, Repositories, UnitOfWork)
builder.Services.AddInfrastructure(builder.Configuration);

// Register Application layer (Services & Business Logic)
builder.Services.AddApplication();

var app = builder.Build();

// Seed the database with test data (Initial users/roles)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<FinalProject.Infrastructure.DbContext.ApplicationDbContext>();
        var seeder = new FinalProject.Infrastructure.Seeding.DataSeeder(context);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        // Log error if seeding fails
        Console.WriteLine($"Database Seeding Error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware order is critical for Identity/Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Required if using Identity Scaffolding/Razor Pages
app.MapRazorPages();

app.Run();