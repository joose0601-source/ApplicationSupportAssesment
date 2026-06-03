using MercadoVerde.Domain;

namespace MercadoVerde.Application.Abstractions;

/// <summary>Acceso de lectura al catálogo de productos.</summary>
public interface IProductoRepository
{
    List<Producto> BuscarPorNombre(string termino);
    Producto? ObtenerPorId(int id);
}
