using WebApplication1.Data;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<WebSocketService>();

var app = builder.Build();

// Creates if Database doesn't exist
DataContext.InitializeDatabase();

// Gets instuments from the service and Adds to table,
// Otherwise websocket service will give foreignKey error.
await DataContext.GetInstrumentsAndAddToTable();

// start websocket, subscribe, and writes data to database
WebSocketService wService = new WebSocketService();
wService.StartWebSocketService(app);

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        await context.Response.WriteAsync("Service Running");
    }
    else
    {
        await next();
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
