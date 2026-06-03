using Microsoft.EntityFrameworkCore;
using MercadoVerde.Application.Abstractions;
using MercadoVerde.Application.Services;
using MercadoVerde.Infrastructure.Data;
using MercadoVerde.Infrastructure.Payments;

// Compatibilidad de fechas: trata los DateTime sin distinción de Kind como
// 'timestamp without time zone' (comportamiento clásico). Evita excepciones de
// Npgsql al portar el código desde SQLite y conserva el comportamiento original.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Las respuestas se serializan en PascalCase (sin política de nombres) para
// coincidir con los contratos que consume el panel de soporte (frontend).
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---- CORS: el panel de soporte (frontend) corre en otro origen ----
// El origen permitido es configurable (Cors:AllowedOrigin / env Cors__AllowedOrigin)
// para que docker compose lo mantenga en sincronía con el puerto del panel.
var corsOrigin = builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:3000";
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(corsOrigin).AllowAnyHeader().AllowAnyMethod()));

// ---- Infraestructura: base de datos PostgreSQL ----
// La cadena de conexión viene de configuración (ConnectionStrings:Default) o de
// la variable de entorno ConnectionStrings__Default (la inyecta docker compose).
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Host=localhost;Port=5432;Database=mercadoverde;Username=mercadoverde;Password=mercadoverde";
builder.Services.AddDbContext<TiendaDbContext>(opt =>
    opt.UseNpgsql(connectionString));
builder.Services.AddScoped<ITiendaDbContext>(sp => sp.GetRequiredService<TiendaDbContext>());
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddSingleton<IPasarelaPagoService, PasarelaPagoService>();

// ---- Aplicación: servicios de orquestación ----
builder.Services.AddScoped<InventarioService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<ReporteService>();

var app = builder.Build();

// Crear y sembrar la base al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    SeedData.Inicializar(db);
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.Run();

/// <summary>Punto de entrada expuesto para pruebas de integración.</summary>
public partial class Program;
