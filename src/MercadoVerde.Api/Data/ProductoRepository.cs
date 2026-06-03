using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MercadoVerde.Api.Models;

namespace MercadoVerde.Api.Data
{
    public class ProductoRepository
    {
        private readonly TiendaDbContext _db;

        public ProductoRepository(TiendaDbContext db)
        {
            _db = db;
        }

        // Búsqueda de productos por nombre para el catálogo público.
        // El término de búsqueda llega directamente desde la query string del usuario.
        public List<Producto> BuscarPorNombre(string termino)
        {
            // Se arma la consulta SQL concatenando el texto recibido del usuario.
            var sql = "SELECT * FROM Productos WHERE Activo = 1 AND Nombre LIKE '%" + termino + "%'";
            return _db.Productos.FromSqlRaw(sql).ToList();
        }

        public Producto? ObtenerPorId(int id) => _db.Productos.FirstOrDefault(p => p.Id == id);
    }
}
