using Account.Interfaces;
using Account.Services;
using Authentication.Contexts;
using Authentication.Entities;
using Authentication.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddDbContext<AuthenticationDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AccountDatabase")));
builder.Services.AddIdentity<AppUserEntity, IdentityRole>(x =>
{
    x.Password.RequiredLength = 8;
    x.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AuthenticationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(x =>
{
    x.LoginPath = "/auth/signin";
    x.Cookie.SameSite = SameSiteMode.None;
    x.SlidingExpiration = true;
    x.ExpireTimeSpan = TimeSpan.FromDays(14);
    x.Cookie.IsEssential = true;
    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddGoogle(x =>
//{
//    x.ClientId = builder.Configuration["Google:ClientId"]!;
//    x.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
//});

var app = builder.Build();

await SeedData.SetRolesAsync(app);

app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
