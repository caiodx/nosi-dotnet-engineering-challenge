using NOS.Engineering.Challenge.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/apilogs-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();
    
app.Run();