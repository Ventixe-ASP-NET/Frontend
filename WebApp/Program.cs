using System.Net.Http.Headers;
using WebApp.Services.Event;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var eventBaseApi = builder.Configuration["EventApi:BaseEventUrl"];
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

builder.Services.AddScoped<IEventService, HttpEventService>();


var app = builder.Build();



app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
