using Microsoft.AspNetCore.Builder;
using FinalProject.Application;
using FinalProject.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
<<<<<<< mahmoud_hany
using FinalProject.Domain.Entities;
=======
using Microsoft.EntityFrameworkCore;
using FinalProject.Web.Data;
using FinalProject.Web.Areas.Identity.Data;
>>>>>>> master

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

<<<<<<< mahmoud_hany
// Register Infrastructure layer (DbContext, Identity, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);
=======
builder.Services.AddDbContext<FinalProjectWebContext>(options =>
    options.UseSqlServer(connectionString));
>>>>>>> master

// ================= IDENTITY =================
builder.Services.AddIdentity<FinalProjectWebUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<FinalProjectWebContext>()
.AddDefaultTokenProviders();

<<<<<<< mahmoud_hany
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "SalahlyAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});
=======
// ================= GOOGLE LOGIN =================
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });
>>>>>>> master

// ================= MVC =================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

<<<<<<< mahmoud_hany
// Seed the database with test data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<FinalProject.Infrastructure.DbContext.ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    var seeder = new FinalProject.Infrastructure.Seeding.DataSeeder(context, userManager);
    await seeder.SeedAsync();
}
=======
// ================= LAYERS =================
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
>>>>>>> master

var app = builder.Build();

// ================= PIPELINE =================
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

<<<<<<< mahmoud_hany
app.Run();
=======
app.MapRazorPages();

app.Run();
>>>>>>> master
