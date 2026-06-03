using Microsoft.EntityFrameworkCore;
using MercadoVerde.Api.Data;
using MercadoVerde.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de datos SQLite local (archivo tienda.db)
builder.Services.AddDbContext<TiendaDbContext>(opt =>
    opt.UseSqlite("Data Source=tienda.db"));

builder.Services.AddScoped<ProductoRepository>();
builder.Services.AddScoped<InventarioService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<ReporteService>();
builder.Services.AddSingleton<PasarelaPagoService>();

var app = builder.Build();

// Crear y sembrar la base al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    SeedData.Inicializar(db);
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
