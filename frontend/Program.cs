using GloboTicket.Frontend.Services;
using GloboTicket.Frontend.Models;
using GloboTicket.Frontend.Services.Ordering;
using GloboTicket.Frontend.Services.ShoppingBasket.Dapr;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IEventCatalogService, EventCatalogService>((sp, c) =>
    c.BaseAddress = new Uri(sp.GetService<IConfiguration>()["ApiConfigs:EventCatalog:Uri"]));
builder.Services.AddScoped<IShoppingBasketService, DaprClientStateStoreShoppingBasket>(); // Using Dapr state store for shopping basket
builder.Services.AddHttpClient<IOrderSubmissionService, HttpOrderSubmissionService>((sp, c) =>
    c.BaseAddress = new Uri(sp.GetService<IConfiguration>()["ApiConfigs:Ordering:Uri"]));

builder.Services.AddSingleton<Settings>();
builder.Services.AddDaprClient();

var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");


builder.Services.AddHttpClient<IEventCatalogService, EventCatalogService>(c =>
    c.BaseAddress = new Uri($"http://localhost:{daprPort}/v1.0/invoke/catalog/method/")); // Service invocation via Dapr

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Turning this off to simplify the running in Kubernetes demo
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EventCatalog}/{action=Index}/{id?}");

app.Run();
