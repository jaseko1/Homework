using Hangfire;
using Homework.Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Konfigurace Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage("VašeConnectionString")); // Zde nastavte vaše pøipojení k databázi

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Nastavení Hangfire Dashboard
app.UseHangfireDashboard();

var scope = app.Services.CreateScope();
var hangfireDbContext = scope.ServiceProvider.GetRequiredService<HangfireDbContext>();
var hangfireResolver = scope.ServiceProvider.GetRequiredService<HangfireResolver>();
hangfireDbContext.Database.EnsureCreated();

app.Run();
