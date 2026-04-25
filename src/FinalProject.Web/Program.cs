using FinalProject.Application;
using FinalProject.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Infrastructure layer (DbContext, Repositories, UnitOfWork)
builder.Services.AddInfrastructure(builder.Configuration);

// Register Application layer (Services)
builder.Services.AddApplication();

// Cookie Authentication — Hardened Security
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "SalahlyAuth";
        options.Cookie.HttpOnly = true;                    // Prevent XSS access to cookie
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTPS in prod
        options.Cookie.SameSite = SameSiteMode.Strict;     // Prevent CSRF via cross-site requests
        options.ExpireTimeSpan = TimeSpan.FromHours(8);    // Session expires after 8 hours
        options.SlidingExpiration = true;                   // Renew on activity
    });

var app = builder.Build();

// Seed the database with test data
using (var scope = app.Services.CreateScope())
{
    var seeder = new FinalProject.Infrastructure.Seeding.DataSeeder(
        scope.ServiceProvider.GetRequiredService<FinalProject.Infrastructure.DbContext.ApplicationDbContext>());
    await seeder.SeedAsync();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
