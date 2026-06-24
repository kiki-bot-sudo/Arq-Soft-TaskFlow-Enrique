using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskFlow.Application.Decorators;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Observers;
using TaskFlow.Application.Services;
using TaskFlow.Application.Strategies;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Interfaces;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskFlow API",
        Version = "v1",
        Description = "API RESTful para gestión de tareas y actividades. " +
                     "Patrones GoF aplicados: Strategy, Builder, Decorator, Observer.",
        Contact = new OpenApiContact { Name = "Enrique Zavala" }
    });

    var xmlFile = "TaskFlow.Api.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Observer: registrado como scoped para poder inyectarse
builder.Services.AddScoped<ActivityCompletionObserver>();

// TaskService con Observer inyectado via factory
builder.Services.AddScoped<ITaskService>(sp =>
{
    var repo = sp.GetRequiredService<ITaskRepository>();
    var service = new TaskService(repo);

    // Registrar el observador de completado automático
    var observer = sp.GetRequiredService<ActivityCompletionObserver>();
    service.AddObserver(observer);

    return service;
});

// ActivityService con Decorator de logging encima
builder.Services.AddScoped<IActivityService>(sp =>
{
    var repo = sp.GetRequiredService<IActivityRepository>();
    var logger = sp.GetRequiredService<ILogger<LoggingActivityServiceDecorator>>();

    // Servicio real (con Strategy por defecto: PriorityDesc)
    var realService = new ActivityService(repo);

    // Decorator envuelve el servicio real añadiendo logging
    return new LoggingActivityServiceDecorator(realService, logger);
});

builder.Services.AddLogging();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskFlow API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
