using MagicVilla_VillaAPI.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().
//    WriteTo.File("log/villaLogs3.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers(option => { option.ReturnHttpNotAcceptable = true; }).AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//maximum and longest time is singleton 
//works when applicaiton starts and every application request 

//addscoped
//for every request

//add transient
//every time that object is accessed, even one request
//is accessed like ten request it will create ten different objects and assign that where it's needed

//uygulama çalıştığında baştan sona bir kez loglama yapması için singelton kullanılmakta
//builder.Services.AddSingleton<ILogging, LoggingV2>();

var app = builder.Build();

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
