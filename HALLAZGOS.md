HALLAZGOS.md

PRUEBA TÉCNICA — SOPORTE DE SISTEMAS .NET
Proyecto MercadoVerde

Candidato: José Saúl Umaña
Rama: solucion-jose-saul
Fecha de inicio: 2026-08-27


PARTE 0 — CONOCE EL SISTEMA

0.1 Arquitectura

El sistema está organizado por capas:

MercadoVerde.Domain
        ↓
MercadoVerde.Application
        ↓
MercadoVerde.Infrastructure
        ↓
MercadoVerde.WebApi

También existen proyectos de pruebas:

tests/
├── MercadoVerde.Application.UnitTests
└── MercadoVerde.WebApi.IntegrationTests

Responsabilidades:

Domain:
Entidades y conceptos centrales del dominio.

Application:
Casos de uso y lógica de aplicación.

Infrastructure:
Persistencia con EF Core/PostgreSQL y servicios externos.

WebApi:
Controladores, endpoints HTTP y configuración de la API.

UnitTests:
Validación aislada de comportamiento.

IntegrationTests:
Validación del comportamiento integrado de la API.


0.2 Flujo de POST /api/pedidos

El flujo observado en PedidoService.CrearPedido es:

1. Buscar el cliente.
2. Validar que exista.
3. Crear el pedido en estado Pendiente.
4. Buscar cada producto.
5. Crear las líneas del pedido.
6. Calcular subtotal.
7. Buscar y validar el cupón.
8. Calcular descuento.
9. Calcular impuesto y total.
10. Ejecutar el cobro mediante IPasarelaPagoService.
11. Actualizar el estado según el resultado del cobro.
12. Descontar inventario.
13. Generar comprobante.
14. Persistir el pedido.
15. Devolver el pedido.


PARTE 1 — TRIAGE Y PRIORIZACIÓN

TICK-205:
Severidad: Crítica.
Impacto: Riesgo financiero e integridad de estados de pago.
Urgencia: Inmediata.
Contención: Evitar nuevas confirmaciones incorrectas y revisar pedidos afectados.
Escalamiento: Finanzas / pagos / backend.

TICK-206:
Severidad: Crítica.
Impacto: Riesgo de seguridad por entrada no confiable en SQL.
Urgencia: Inmediata.
Contención: Revisar exposición y mitigar el buscador si fuera necesario.
Escalamiento: Seguridad / backend.

TICK-201:
Severidad: Alta/Crítica.
Impacto: Venta de inventario inexistente y afectación de entregas.
Urgencia: Inmediata.
Contención: Revisar stock afectado y controlar operaciones concurrentes.
Escalamiento: Operaciones / backend.

TICK-203:
Severidad: Alta.
Impacto: Errores 500 en escenarios específicos.
Urgencia: Alta.
Contención: Reproducir con cliente/cupón afectados y conservar evidencia.
Escalamiento: Backend / soporte.

TICK-204:
Severidad: Alta.
Impacto: Reporte cercano al timeout.
Urgencia: Alta.
Contención: Reducir temporalmente rango/volumen consultado.
Escalamiento: Backend / infraestructura.

TICK-202:
Severidad: Alta.
Impacto: Totales e impuestos incorrectos.
Urgencia: Alta.
Contención: No usar cálculos actuales como fuente financiera confiable.
Escalamiento: Finanzas / backend.

TICK-301:
Severidad: Alta.
Impacto: Comportamiento inesperado del buscador del panel.
Urgencia: Alta.
Contención: Validar entrada y API consumida.
Escalamiento: Seguridad / frontend-backend.

TICK-302:
Severidad: Alta.
Impacto: Montos del panel no coinciden con cobro real.
Urgencia: Alta.
Contención: No tomar el cálculo del panel como fuente de verdad.
Escalamiento: Finanzas / frontend-backend.

TICK-303:
Severidad: Alta.
Impacto: Posibles pedidos duplicados.
Urgencia: Alta.
Contención: Bloquear acción mientras está en proceso.
Escalamiento: Frontend/backend.

TICK-305:
Severidad: Alta.
Impacto: El panel puede mostrar éxito cuando la operación falla.
Urgencia: Alta.
Contención: Corregir manejo de errores y estados visuales.
Escalamiento: Frontend/backend.

TICK-304:
Severidad: Media/Alta.
Impacto: Reporte puede congelar el navegador.
Urgencia: Alta.
Contención: Reducir volumen mostrado/paginar.
Escalamiento: Frontend.

TICK-306:
Severidad: Media/Alta.
Impacto: Se omiten registros del límite superior del rango.
Urgencia: Alta.
Contención: Revisar rangos antes de usar el reporte.
Escalamiento: Frontend/backend.

TICK-307:
Severidad: Media/Alta.
Impacto: Búsqueda lenta y respuestas fuera de orden.
Urgencia: Media/Alta.
Contención: Evitar aplicar respuestas obsoletas.
Escalamiento: Frontend.

TICK-308:
Severidad: Media.
Impacto: Badge de stock incorrecto.
Urgencia: Media.
Contención: Ajustar umbral visual.
Escalamiento: Frontend / operaciones.

TICK-309:
Severidad: Alta.
Impacto: Se aceptan cantidades inválidas.
Urgencia: Alta.
Contención: Validar también en backend.
Escalamiento: Backend / frontend.


PARTE 2 — DIAGNÓSTICO

TICK-201 — Concurrencia de inventario

Síntoma:
Dos pedidos casi simultáneos pueden vender más unidades de las disponibles y producir inventario inconsistente.

Evidencia:
InventarioService.DescontarStock realiza:

var producto = _db.Productos.FirstOrDefault(p => p.Id == productoId);

if (producto.Stock < cantidad)
    throw new InvalidOperationException(...);

producto.Stock = producto.Stock - cantidad;

_db.SaveChanges();

La validación y la actualización están separadas.

Producto también declara:

public byte[]? RowVersion { get; set; }

Sin embargo, TiendaDbContext no configura RowVersion como mecanismo de concurrencia.

En PostgreSQL se comprobó que RowVersion es de tipo bytea y los registros existentes no tenían un valor útil.

Hipótesis de causa raíz:
Condición de carrera entre lectura, validación y actualización del stock.

Reproducción controlada:
Usar un producto con stock 1 y ejecutar dos solicitudes concurrentes de cantidad 1.

Resultado esperado después de corregir:
Una operación debe descontar stock y la otra debe rechazarse. Nunca debe quedar Stock < 0.

Estado:
Diagnosticado. Corrección implementada; pendiente de validación concurrente.


TICK-202 — Impuesto, redondeo y vigencia del cupón

Síntoma:
Con un cupón válido, el impuesto se calculaba sobre el subtotal completo y la vigencia del cupón utilizaba una referencia temporal distinta de UTC. Los importes podían quedar con más de dos decimales.

Evidencia inicial:

if (cupon.FechaExpiracionUtc >= DateTime.Now && cupon.Activo)
{
    descuento = subtotal * (cupon.PorcentajeDescuento / 100m);
}

y:

decimal impuesto = subtotal * TasaImpuesto;
decimal total = subtotal - descuento + impuesto;

El test proporcionado por el repositorio establece:

Subtotal       = 100.00
Descuento      = 10.00
Base imponible = 90.00
Impuesto       = 11.70
Total          = 101.70

Una prueba previa de API produjo:

Subtotal = 49.90
Descuento = 4.99
Impuesto = 6.487
Total = 51.397

Causa raíz:
1. Se utilizaba DateTime.Now para comparar una fecha almacenada en UTC.
2. El impuesto se calculaba sobre subtotal en lugar de subtotal - descuento.
3. No existía redondeo explícito a dos decimales.

Corrección aplicada:

cupon.FechaExpiracionUtc >= DateTime.UtcNow

y:

decimal baseImponible = subtotal - descuento;

decimal impuesto = Math.Round(
    baseImponible * TasaImpuesto,
    2,
    MidpointRounding.AwayFromZero
);

decimal total = Math.Round(
    baseImponible + impuesto,
    2,
    MidpointRounding.AwayFromZero
);

El descuento también se redondea a dos decimales.

Validación automática:
Se activó el test:
TICK202_Impuesto_SeCalculaSobreSubtotalConDescuento

Comando:

dotnet test tests/MercadoVerde.Application.UnitTests/MercadoVerde.Application.UnitTests.csproj --no-restore

Resultado:

Superado: 2
Con error: 0
Omitido: 3
Total: 5

El test de TICK-202 pasó correctamente.

Validación manual por API:
Después de reconstruir la API con:

docker compose up -d --build api

se realizó una prueba con Postman contra:

POST http://localhost:5080/api/Pedidos

usando el cupón BIENVENIDA10 y un producto con stock disponible.

Resultado:

Subtotal = 25.00
Descuento = 2.50
Impuesto = 2.93
Total = 25.43

Cálculo:

(25.00 - 2.50) × 13% = 2.925 → 2.93
25.00 - 2.50 + 2.93 = 25.43

La respuesta de la API confirmó que el pedido fue creado y el stock disminuyó correctamente.

Regresión potencial:
El cambio afecta el cálculo de importes de pedidos con cupón. Se mantiene cubierto por prueba automatizada y validación manual.

Estado:
Resuelto y validado.


TICK-203 — Cupón inexistente / errores 500

Síntoma:
Un código de cupón inexistente puede provocar un error 500.

Evidencia:
FirstOrDefault puede devolver null y el código original accedía directamente a FechaExpiracionUtc.

El test utiliza el código PROMO50 como cupón inexistente y exige que no se produzca NullReferenceException.

Además, SeedData contiene un cliente sin email y GenerarLineaComprobante usa cliente.Email.ToUpper(), lo que constituye otra posible fuente de NullReferenceException.

Estado:
Diagnosticado. Pendiente de validación con el escenario completo.


TICK-204 — Reporte lento

Síntoma:
El reporte tarda demasiado y puede expirar.

Evidencia:
La implementación realiza consultas adicionales de líneas y clientes dentro de un ciclo sobre los pedidos.

Hipótesis:
Patrón N+1 y exceso de materialización.

Estado:
Diagnosticado. Pendiente de corrección y medición.


TICK-205 — Pedido marcado como Pagado sin confirmación

Síntoma:
Un pedido puede quedar Pagado aunque la pasarela haya fallado.

Evidencia:
El código original contiene:

catch
{
    pedido.Estado = EstadoPedido.Pagado;
}

El test esperado exige que una caída de la pasarela nunca deje el pedido en Pagado.

Causa raíz:
Una falla técnica de la pasarela se estaba interpretando como aprobación del cobro.

Estado:
Diagnosticado. Pendiente de corrección.


TICK-206 — Inyección SQL en buscador

Síntoma:
Ciertos caracteres pueden romper la búsqueda y una entrada manipulada puede alterar el resultado.

Evidencia:
ProductoRepository.BuscarPorNombre concatena directamente el término recibido en SQL y ejecuta FromSqlRaw.

El test espera:

"x'" → no debe romper.
"' OR 1=1 --" → no debe devolver todo el catálogo.

Estado:
Diagnosticado. Pendiente de corrección y validación.


PARTE 3 — CORRECCIONES

TICK-201 — Corrección implementada; pendiente de validación final.
TICK-202 — Resuelto y validado.
TICK-203 — Pendiente.
TICK-204 — Pendiente.
TICK-205 — Pendiente.
TICK-206 — Pendiente.
TICK-301 — Pendiente.
TICK-302 — Pendiente.
TICK-303 — Pendiente.
TICK-304 — Pendiente.
TICK-305 — Pendiente.
TICK-306 — Pendiente.
TICK-307 — Pendiente.
TICK-308 — Pendiente.
TICK-309 — Pendiente.


PARTE 4 — CAMBIO DE REQUERIMIENTO

Límite máximo de descuento de $15.00

Regla:
Ningún cupón puede generar un descuento superior a $15.00 por pedido.

Si el descuento calculado supera $15.00:

Descuento aplicado = $15.00

La regla debe estar en la lógica de negocio/aplicación y cubierta mediante pruebas automatizadas.

También se debe dejar constancia útil para soporte cuando el descuento haya sido limitado.

Estado:
Pendiente de implementación.


PARTE 5 — COMUNICACIÓN Y CIERRE

RCA:
Pendiente.

Estructura:
- Causa raíz
- Impacto
- Corrección
- Acción preventiva

Mensaje al área de negocio:
Pendiente. Debe ser de 3 a 5 líneas y sin jerga técnica.


EVIDENCIA DE EJECUCIÓN

2026-08-27 — TICK-202

Comando:
dotnet test tests/MercadoVerde.Application.UnitTests/MercadoVerde.Application.UnitTests.csproj --no-restore

Resultado:
Superado: 2
Con error: 0
Omitido: 3
Total: 5

Prueba manual:
POST http://localhost:5080/api/Pedidos

Resultado:
Subtotal = 25.00
Descuento = 2.50
Impuesto = 2.93
Total = 25.43


REGISTRO DE CAMBIOS

2026-08-27 — TICK-201
Cambio:
Corrección de descuento de stock con actualización atómica condicionada.

Validación:
Pendiente de prueba concurrente.

Commit:
Pendiente.

2026-08-27 — TICK-202
Cambio:
Corrección de vigencia UTC, base del impuesto y redondeo monetario.

Validación:
Test automático + prueba manual mediante Postman.

Commit:
Pendiente.
