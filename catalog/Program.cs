using GloboTicket.Catalog.Repositories;
using GloboTicket.Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IEventRepository, EventRepository>();

var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");

builder.Services.AddHttpClient<IEventCatalogService>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
