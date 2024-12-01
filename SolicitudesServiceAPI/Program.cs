using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using SolicitudesService.Infrastructure.Data;
using SolicitudesService.Interfaces;
using SolicitudesService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy.WithOrigins("http://localhost:8088", "http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// Configurar DbContext y conexión a SQL Server
builder.Services.AddDbContext<SolicitudesServiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SolicitudesServiceDb"),
    sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("SolicitudesService.Infrastructure");
        sqlOptions.EnableRetryOnFailure();
    }));

// Configurar HttpClient para FuncionarioService
builder.Services.AddHttpClient("FuncionarioService", client =>
{
    var funcionarioServiceUrl = builder.Configuration["FuncionarioServiceUrl"];
    if (string.IsNullOrEmpty(funcionarioServiceUrl))
    {
        throw new InvalidOperationException("FuncionarioServiceUrl is not configured in the application settings.");
    }
    client.BaseAddress = new Uri(funcionarioServiceUrl);
});

// Registrar los servicios de la aplicación
builder.Services.AddScoped<ISolicitudDocumentoService, SolicitudDocumentoService>();
builder.Services.AddScoped<ISolicitudHorasExtraService, SolicitudHorasExtraService>();
builder.Services.AddScoped<ISolicitudPersonalService, SolicitudPersonalService>();
builder.Services.AddScoped<ISolicitudVacacionesService, SolicitudVacacionesService>();

// Configurar Swagger para la documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SolicitudesService API",
        Version = "v1",
        Description = "API para la gestión de solicitudes de vacaciones, horas extra, y documentos.",
        Contact = new OpenApiContact
        {
            Name = "Tu Nombre",
            Email = "tuemail@correo.com",
            Url = new Uri("http://localhost:8088/api/SolicitudesServiceAPI")
        }
    });
});

// Configurar controladores y habilitar el manejo de referencias cíclicas
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

var app = builder.Build();

// Activar CORS antes de los controladores y otras configuraciones
app.UseCors("AllowLocalhost");

// Aplicar migraciones automáticamente con lógica de reintento
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SolicitudesServiceDbContext>();

    const int maxRetries = 10; // Número máximo de intentos
    const int delayInSeconds = 5; // Tiempo entre intentos

    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            Console.WriteLine($"Intentando aplicar migraciones. Intento {attempt}/{maxRetries}...");
            dbContext.Database.Migrate();
            Console.WriteLine("Migraciones aplicadas con éxito.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al aplicar migraciones: {ex.Message}");

            if (attempt == maxRetries)
            {
                Console.WriteLine("No se pudo aplicar las migraciones después de varios intentos. Cerrando aplicación.");
                throw; // Lanza la excepción si todos los intentos fallan
            }

            Console.WriteLine($"Reintentando en {delayInSeconds} segundos...");
            await Task.Delay(delayInSeconds * 1000); // Espera antes de reintentar
        }
    }
}

// Configurar Swagger y controladores
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SolicitudesService API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
