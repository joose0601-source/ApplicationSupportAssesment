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

// Se mantiene el mismo cálculo y redondeo que el backend.
export function calcularTotalEstimado(
  lineas: LineaResumen[],
  porcentajeCupon: number
): number {
  const subtotal = calcularSubtotal(lineas);

  const descuento = Number(
    (subtotal * (porcentajeCupon / 100)).toFixed(2)
  );

  const baseImponible = subtotal - descuento;

  const impuesto = Number(
    (baseImponible * TASA_IMPUESTO).toFixed(2)
  );

  return Number((baseImponible + impuesto).toFixed(2));
}

// Formatea un monto para mostrarlo en la interfaz.
export function formatearMoneda(monto: number): string {
  return "$" + monto.toFixed(2);
}
