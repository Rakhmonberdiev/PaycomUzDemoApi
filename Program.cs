using PaycomUz.Abstractions;
using PaycomUz.Configuration;
using PaycomUz.Middleware;
using PaycomUz.Services;
using PaycomUzDemoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MockDataService>();

builder.Services.AddScoped<IPaycomService, PaycomService>();
builder.Services.Configure<PaycomSettings>(builder.Configuration.GetSection("Paycom"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<PaycomMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.Run();


