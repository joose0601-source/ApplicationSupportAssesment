using System.Linq;
using MercadoVerde.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MercadoVerde.WebApi.IntegrationTests;

/// <summary>
/// Arranca la aplicación real pero sustituye PostgreSQL por SQLite en memoria,
/// para que la prueba de humo corra en cualquier máquina sin un Postgres vivo.
/// La conexión se mantiene abierta durante toda la vida de la factory.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // Quitar el DbContext configurado con Npgsql.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TiendaDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // Registrar el mismo DbContext sobre SQLite en memoria.
            services.AddDbContext<TiendaDbContext>(opt => opt.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }
}
