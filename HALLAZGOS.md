HALLAZGOS.md

PRUEBA TÉCNICA — SOPORTE DE SISTEMAS .NET
Proyecto MercadoVerde

Candidato: José Saúl Umaña
Rama: solucion-jose-saul
Fecha de inicio: 2026-08-27

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


## TICK-201 — Concurrencia de inventario

**Síntoma:** Dos pedidos simultáneos podían vender más stock del disponible.
**Evidencia:** La validación y descuento del stock se hacían en operaciones separadas.
**Causa raíz:** Condición de carrera al leer y actualizar el inventario.
**Corrección:** Se implementó un descuento atómico condicionado por `Stock >= cantidad`.
**Cambio:** La operación se centralizó en `ITiendaDbContext` y `TiendaDbContext`.
**Resultado:** Si no hay stock suficiente, la operación es rechazada.
**Prueba:** Se dejó el producto con stock inicial de 1 y se enviaron dos pedidos simultáneos.
**Resultado obtenido:** Un pedido fue creado y el segundo fue rechazado por falta de stock.
**Validación:** El stock final en PostgreSQL quedó en `0`, nunca en `-1`.
**Estado:** Resuelto y validado.


## TICK-202 — Impuesto, redondeo y vigencia del cupón

**Síntoma:** El impuesto se calculaba sobre el subtotal completo y la vigencia usaba `DateTime.Now`.
**Evidencia:** El código calculaba `subtotal * TasaImpuesto` y podía producir importes con más de dos decimales.
**Causa raíz:** Uso incorrecto de zona horaria, base imponible incorrecta y falta de redondeo.
**Corrección:** Se cambió `DateTime.Now` por `DateTime.UtcNow`.
**Cambio:** El impuesto ahora se calcula sobre `subtotal - descuento`.
**Cambio:** Descuento, impuesto y total se redondean a dos decimales.
**Prueba:** Se activó `TICK202_Impuesto_SeCalculaSobreSubtotalConDescuento`.
**Resultado:** `Superado: 2`, `Con error: 0`, `Omitido: 3`, `Total: 5`.
**Validación API:** Postman devolvió `Subtotal 25.00`, `Descuento 2.50`, `Impuesto 2.93`, `Total 25.43`.
**Estado:** Resuelto y validado.

## TICK-203 — Error 500 con cupón PROMO50

**Síntoma:** Algunos pedidos devolvían HTTP 500 con un cliente específico y el cupón `PROMO50`.
**Evidencia:** El log mostró `NullReferenceException` en `PedidoService.GenerarLineaComprobante`, al utilizar el email del cliente. :contentReference[oaicite:0]{index=0}
**Causa raíz:** El cliente podía tener `Email = null` y se ejecutaba `cliente.Email.ToUpper()`.
**Corrección:** Se agregó manejo seguro del email nulo usando `?.` y un valor por defecto.
**Prueba:** Se activó `TICK203_CuponInexistente_NoLanzaNullReference`.
**Resultado:** `Superado: 3`, `Con error: 0`, `Omitido: 2`.
**Validación API:** Postman con cliente 2 y `PROMO50` devolvió HTTP 200.
**Resultado:** Pedido creado con `Descuento = 0`, `Impuesto = 3.25` y `Total = 28.25`.
**Regresión:** Se mantiene la validación del flujo cuando el cliente no tiene email.
**Estado:** Resuelto y validado.

## TICK-204 — Reporte lento

**Síntoma:** El reporte podía volverse lento con gran cantidad de pedidos.
**Evidencia:** Se detectó un patrón N+1 al consultar líneas y clientes dentro de un ciclo.
**Causa raíz:** Consultas adicionales por cada pedido.
**Corrección:** Se reemplazó el procesamiento por una consulta LINQ que genera JOIN y agregación en SQL.
**Validación:** El endpoint devolvió correctamente los pedidos, clientes, cantidades y totales.
**Resultado:** Los logs muestran una consulta combinada para el reporte en lugar de consultas repetidas por pedido.
**Tests:** `Superado: 3`, `Con error: 0`, `Omitido: 2`.
**API:** Validada después de reconstruir Docker.
**Resultado final:** El reporte mantiene la información correcta y reduce las consultas innecesarias.
**Estado:** Resuelto y validado.

## TICK-205 — Pedido marcado como Pagado cuando falla la pasarela

**Síntoma:** Un pedido podía quedar `Pagado` cuando la pasarela de pago fallaba.
**Evidencia:** El test mostró que una excepción de la pasarela dejaba `EstadoPedido.Pagado`.
**Causa raíz:** El `catch` trataba un fallo técnico como un pago aprobado.
**Corrección:** Se cambió el manejo del `catch` para dejar el pedido en `Rechazado`.
**Prueba:** Se activó `TICK205_CobroQueFalla_NoDejaPedidoPagado`.
**Resultado inicial:** El test falló con `EstadoPedido.Pagado`.
**Resultado final:** `Superado: 4`, `Con error: 0`, `Omitido: 1`.
**Comportamiento esperado:** Un fallo de la pasarela nunca debe marcar el pedido como `Pagado`.
**Regresión:** Los tests de TICK-202 y TICK-203 continúan pasando.
**Estado:** Resuelto y validado.