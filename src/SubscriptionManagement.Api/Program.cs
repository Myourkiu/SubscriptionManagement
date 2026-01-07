using Microsoft.EntityFrameworkCore;
using SubscriptionManagement.Infrastructure.Data;
using SubscriptionManagement.Infrastructure.Repositories;
using SubscriptionManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<SubscriptionRepository>();
builder.Services.AddScoped<PlanRepository>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<PlanService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Redireciona a rota raiz para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapControllers();

app.Run();
