using FinalProject.Application;
using FinalProject.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinalProject.Web.Data;
using FinalProject.Web.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FinalProjectWebContext>(options =>
    options.UseSqlServer(connectionString));

// ================= IDENTITY =================
builder.Services.AddDefaultIdentity<FinalProjectWebUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<FinalProjectWebContext>();

// ================= GOOGLE LOGIN =================
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

// ================= MVC =================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ================= LAYERS =================
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

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

app.MapRazorPages();

app.Run();
