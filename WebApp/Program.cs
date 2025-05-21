
using Account.Contexts;
using Account.Entities;
using Account.Interfaces;
using Account.SeedData;
using Account.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using WebApp.Services.Event;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


var eventBaseApi = builder.Configuration["EventApi:BaseEventUrl"];
var bookingGatewayBaseApi = builder.Configuration["BookingGateway:BaseUrl"];
var bookingBaseApi = builder.Configuration["BookingApi:BaseUrl"];
if (string.IsNullOrEmpty(eventBaseApi))
{
    throw new ArgumentNullException("EventApi:BaseEventUrl", "BaseEventUrl is not set in appsettings.json");
}






builder.Services.AddHttpClient("EventApi", client =>
{
    client.BaseAddress = new Uri(eventBaseApi);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("BookingGateway", client =>
{
    client.BaseAddress = new Uri(bookingGatewayBaseApi);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("BookingApi", client =>
{
    client.BaseAddress = new Uri(bookingBaseApi);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("invoiceGateway", c =>
{
    c.BaseAddress = new Uri("https://invoiceservice-gtg3ajc5htdcgpgs.swedencentral-01.azurewebsites.net");
});

builder.Services.AddScoped<IEventService, HttpEventService>();





//builder.Services.AddHttpClient("bookingGateway", c =>
//{
//    c.BaseAddress = new Uri("https://bookingeventgateway-f8b4d2ahagc5faev.swedencentral-01.azurewebsites.net/");
//});

//builder.Services.AddHttpClient("eventApi", c =>
//{
//    c.BaseAddress = new Uri("https://ventixe-event-rest-api-cxeqehfrcqcvdkck.swedencentral-01.azurewebsites.net/");
//});
//builder.Services.AddHttpClient("bookingApi", c =>
//{
//    c.BaseAddress = new Uri("https://bookingserviceventixe-fbb7amdzfsh4b4d6.swedencentral-01.azurewebsites.net");
//});


builder.Services.AddHttpClient();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddDbContext<AccountDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AccountDatabase")));
builder.Services.AddIdentity<AppUserEntity, IdentityRole>(x =>
{
    x.Password.RequiredLength = 8;
    x.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AccountDbContext>()
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
