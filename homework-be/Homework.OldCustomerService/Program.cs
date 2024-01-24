using Homework.OldCustomerService.Attributes;
using Homework.OldCustomerService.Data;
using Homework.OldCustomerService.Services;
using Homework.OldCustomerService.Services.Interfaces;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSoapCore();
builder.Services.AddSingleton<IRequestCounter, RequestCounter>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Scan(scan => scan
        .FromApplicationDependencies()
        .AddClasses(o => o.WithAttribute<ScopedAttribute>())
        .AsImplementedInterfaces()
        .WithScopedLifetime());

// Registrace MongoDB kontextu
builder.Services.AddScoped<OldServiceDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRouting();

app.UseEndpoints(endpoints => {
    _ = endpoints.UseSoapEndpoint<ICustomerService>("/Customer.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
});

app.MapControllers();

app.Run();
