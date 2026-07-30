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
using TaskFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

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
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        if (builder.Environment.IsDevelopment() && origins.Length == 0)
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        else if (origins.Length > 0)
            policy.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se configuró la conexión DefaultConnection.");
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddEntityFrameworkStores<TaskFlowDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "TaskFlow.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        context.Response.Redirect("/login.html");
        return Task.CompletedTask;
    };
});

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
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.Use(async (context, next) =>
{
    if ((context.Request.Path == "/" || context.Request.Path == "/index.html")
        && context.User.Identity?.IsAuthenticated != true)
    {
        context.Response.Redirect("/login.html");
        return;
    }
    await next();
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapFallbackToFile("index.html").RequireAuthorization();

app.Run();
