using GloboTicket.Catalog.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IEventRepository, EventRepository>();

builder.Services.AddControllers().AddDapr();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
