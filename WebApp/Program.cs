using Account.Interfaces;
using Account.Services;
using Authentication.Contexts;
using Authentication.Entities;
using Authentication.SeedData;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGitHub(x =>
    {
        x.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
        x.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
        x.Scope.Add("user:email");
        x.Scope.Add("read:user");
        x.Events.OnCreatingTicket = async context =>
        {
            await Task.Delay(0);
            if (context.User.TryGetProperty("name", out var name))
            {
                var fullName = name.GetString();
                if (!string.IsNullOrEmpty(fullName))
                {
                    var names = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (names.Length > 0)
                    {
                        context.Identity?.AddClaim(new Claim(ClaimTypes.GivenName, names[0]));
                    }

                    if (names.Length > 1)
                    {
                        context.Identity?.AddClaim(new Claim(ClaimTypes.Surname, names[1]));
                    }

                }
            }
        };
    });

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
