var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("bookingGateway", c =>
{
    c.BaseAddress = new Uri("https://bookingeventgateway-f8b4d2ahagc5faev.swedencentral-01.azurewebsites.net/");
});

builder.Services.AddHttpClient("eventApi", c =>
{
    c.BaseAddress = new Uri("https://ventixe-event-rest-api-cxeqehfrcqcvdkck.swedencentral-01.azurewebsites.net/");
});












builder.Services.AddHttpClient();
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
