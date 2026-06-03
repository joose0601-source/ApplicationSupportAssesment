// Utilidades de dinero para el panel de soporte.
//
// Finanzas pidió que el panel muestre un "resumen estimado" del pedido ANTES
// de enviarlo, para que el agente de soporte confirme el monto con el cliente.
// La tasa de impuesto debe coincidir con la del backend (IVA 13%).

export const TASA_IMPUESTO = 0.13;

export interface LineaResumen {
  precioUnitario: number;
  cantidad: number;
}

// Calcula el subtotal del pedido sumando línea por línea.
export function calcularSubtotal(lineas: LineaResumen[]): number {
  let subtotal = 0;
  for (const l of lineas) {
    subtotal += l.precioUnitario * l.cantidad;
  }
  return subtotal;
}

// Calcula el total estimado a cobrar.
// porcentajeCupon llega como número 0..100 (ej. 10 para 10%).
export function calcularTotalEstimado(
  lineas: LineaResumen[],
  porcentajeCupon: number
): number {
  const subtotal = calcularSubtotal(lineas);
  const impuesto = subtotal * TASA_IMPUESTO;
  const descuento = subtotal * (porcentajeCupon / 100);
  return subtotal + impuesto - descuento;
}

// Formatea un monto para mostrarlo en la interfaz.
export function formatearMoneda(monto: number): string {
  return "$" + monto.toFixed(2);
}
