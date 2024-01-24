using Homework.Gateway.Attributes;
using Homework.Gateway.Configurations;
using Homework.Gateway.Data;
using Homework.Gateway.Data.Repositories;
using Homework.Gateway.Data.Repositories.Interfaces;
using Homework.Gateway.Hubs;
using Homework.Gateway.Services;
using Homework.Gateway.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSignalR();

builder.Services.AddDbContextFactory<GatewayDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AppDb");
    options.UseSqlServer(connectionString, sql =>
    {
        sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

builder.Services.AddDbContext<GatewayDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AppDb");
    options.UseSqlServer(connectionString, sql =>
    {
        sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

builder.Services.AddSingleton<IRequestQueueHandler, RequestQueueHandler>();
builder.Services.AddScoped<IQueueRequestRepository, QueueRequestRepository>();
builder.Services.Scan(scan => scan
        .FromApplicationDependencies()
        .AddClasses(o => o.WithAttribute<ScopedAttribute>())
        .AsImplementedInterfaces()
.WithScopedLifetime());

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:3000", "http://localhost:55806", "http://localhost:*")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

});
builder.Services.Configure<OldCustomerServiceConfig>(builder.Configuration.GetSection("OldService"));
var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var context = services.GetRequiredService<GatewayDbContext>();
context.Database.EnsureCreated();

var customerService = services.GetRequiredService<ICustomerService>();
await customerService.FillRequestQueueFromDb();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
