using System.Collections.Generic;

namespace MercadoVerde.Api.Dtos
{
    public class CrearPedidoDto
    {
        public int ClienteId { get; set; }
        public string? CodigoCupon { get; set; }
        public List<LineaPedidoDto> Lineas { get; set; } = new();
    }

    public class LineaPedidoDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
