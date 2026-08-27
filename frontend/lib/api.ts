import type { CrearPedidoDto, FilaReporte, Pedido, Producto } from "./types";

// URL base de la API MercadoVerde.
// Configurable por entorno (NEXT_PUBLIC_API_BASE en .env.local); por defecto
// la API de .NET levanta en http://localhost:5080/swagger (ver README de la raíz).
const API_BASE = process.env.NEXT_PUBLIC_API_BASE ?? "http://localhost:5080";

// Cliente HTTP del panel de soporte.
// Centraliza las llamadas a la API para el catálogo, los pedidos y los reportes.

export async function buscarProductos(termino: string): Promise<Producto[]> {
  try {
    const res = await fetch(`${API_BASE}/api/productos/buscar?termino=${termino}`);
    const data = await res.json();
    return data as Producto[];
  } catch {
    return [];
  }
}
export async function crearPedido(dto: CrearPedidoDto): Promise<Pedido> {
  const res = await fetch(`${API_BASE}/api/pedidos`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dto),
  });

  if (!res.ok) {
    throw new Error(`No se pudo crear el pedido (${res.status}).`);
  }

  return (await res.json()) as Pedido;
}
export async function obtenerReporteVentas(
  desde: string,
  hasta: string
): Promise<FilaReporte[]> {
  try {
    const res = await fetch(
      `${API_BASE}/api/reportes/ventas?desde=${desde}&hasta=${hasta}`
    );
    const data = await res.json();
    return data as FilaReporte[];
  } catch {
    return [];
  }
}
