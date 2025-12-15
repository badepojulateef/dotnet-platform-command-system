using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataServices;
using PlatformService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
var _env = builder.Environment;

// Add services to the container.

// Console.WriteLine("--> Using SQl Server DB");

// var conn = builder.Configuration.GetConnectionString("PlatformsConn");
// Console.WriteLine($"Conn Str --> {conn}");

// Console.WriteLine("--> Using SQl Server DB");
// builder.Services.AddDbContext<AppDBContext>(opt =>
// opt.UseSqlServer(conn)
// );

// mssql-clusterip-srv

if (_env.IsProduction())
{
  Console.WriteLine("--> Using SQl Server DB");
  var conn = builder.Configuration.GetConnectionString("PlatformsConn");
  Console.WriteLine($"Conn Str --> {conn}");

  builder.Services.AddDbContext<AppDBContext>(opt =>
  opt.UseSqlServer(conn)
  );
}
else
{
  Console.WriteLine("--> Using In Mem DB");
  builder.Services.AddDbContext<AppDBContext>(opt =>
    opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
  {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
  });

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

// builder.Services.AddHttpsRedirection(o =>
// {
//   o.HttpsPort = 7258;
// });


// builder.WebHost.ConfigureKestrel(options =>
// {

//   // REST - HTTP
//   options.ListenLocalhost(5219, o =>
//   {
//     o.Protocols = HttpProtocols.Http1;
//     // options.ListenAnyIP(5219);
//   });

//   // options.ListenAnyIP(5219);

//   // REST - HTTPS
//   options.ListenLocalhost(7258, o =>
//   {
//     o.UseHttps();
//     o.Protocols = HttpProtocols.Http1;
//   });

//   // gRPC - HTTPS + HTTP/2
//   options.ListenLocalhost(7259, o =>
//   {
//     o.UseHttps();
//     o.Protocols = HttpProtocols.Http2;
//   });

//   // options.ListenAnyIP(5219); // HTTP
//   // options.ListenAnyIP(7259, listenOptions =>
//   // {
//   //   listenOptions.UseHttps();
//   // });

//   // // HTTPS + HTTP/2 (gRPC)
//   // options.ListenLocalhost(7259, o =>
//   // {
//   //   o.UseHttps();
//   //   o.Protocols = HttpProtocols.Http2;
//   // });

//   // Optional HTTP (dev only)
//   // options.ListenLocalhost(5259, o =>
//   // {
//   //   o.Protocols = HttpProtocols.Http2;
//   // });
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
}

// app.UseHttpsRedirection();

// app.UseRouting();


PrebDB.PrepPopulation(app, _env.IsProduction());



var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
  var forecast = Enumerable.Range(1, 5).Select(index =>
      new WeatherForecast
      (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          summaries[Random.Shared.Next(summaries.Length)]
      ))
      .ToArray();
  return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();

app.MapGet("protos/platforms.proto", async context =>
{
  await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
