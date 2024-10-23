using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization; // Para manejar las referencias c�clicas en JSON
using SolicitudesService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Configurar DbContext y conexi�n a SQL Server
builder.Services.AddDbContext<SolicitudesServiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SolicitudesServiceDb"),
    sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("SolicitudesService.Infrastructure");
        sqlOptions.EnableRetryOnFailure(); // Habilita reintentos autom�ticos en caso de error transitorio
    }));

// Configurar HttpClient para FuncionarioService
builder.Services.AddHttpClient("FuncionarioService", client =>
{
    client.BaseAddress = new Uri("http://funcionario-api:8085/api/Empleado"); // Base URL de FuncionarioService
});

// Configurar Swagger para la documentaci�n de la API sin requerir autenticaci�n
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SolicitudesService API",
        Version = "v1",
        Description = "API para la gesti�n de solicitudes de vacaciones, horas extra, y documentos.",
        Contact = new OpenApiContact
        {
            Name = "Tu Nombre",
            Email = "tuemail@correo.com",
            Url = new Uri("http://localhost:8088/api/SolicitudesServiceAPI")
        }
    });
});

// Configurar controladores y habilitar el manejo de referencias c�clicas
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Manejo de ciclos de referencia
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true; // Opcional, para mejor legibilidad
});

var app = builder.Build();

// Activar CORS antes de los controladores y otras configuraciones
app.UseCors("AllowLocalhost3000");

// Migraciones y creaci�n de base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SolicitudesServiceDbContext>();

    try
    {
        // Aplicar migraciones pendientes para crear o actualizar las tablas
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
        Console.WriteLine("Migraciones aplicadas con �xito.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al aplicar migraciones: {ex.Message}");
    }
}

// Configurar la app para usar controladores y otras funcionalidades de ASP.NET Core
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SolicitudesService API v1");
        c.RoutePrefix = string.Empty; // Hacer que Swagger sea la p�gina principal
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
