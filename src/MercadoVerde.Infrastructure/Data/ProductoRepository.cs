using System.Linq;
using Microsoft.EntityFrameworkCore;
using MercadoVerde.Application.Abstractions;
using MercadoVerde.Domain;

namespace MercadoVerde.Infrastructure.Data;

public class ProductoRepository : IProductoRepository
{
    private readonly TiendaDbContext _db;

    public ProductoRepository(TiendaDbContext db)
    {
        _db = db;
    }

    // Búsqueda de productos por nombre para el catálogo público.
    public List<Producto> BuscarPorNombre(string termino)
    {
        // Busco por nombre sin concatenar SQL del usuario.

        var texto = termino?.ToLowerInvariant() ?? string.Empty;
        var patron = $"%{texto}%";

        return _db.Productos
            .Where(p =>
                p.Activo &&
                EF.Functions.Like(p.Nombre.ToLower(), patron))
            .ToList();
    }

    public Producto? ObtenerPorId(int id) => _db.Productos.FirstOrDefault(p => p.Id == id);
}
