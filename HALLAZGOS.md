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

## TICK-206 — Inyección SQL en buscador

**Síntoma:** La búsqueda podía fallar con caracteres especiales y ser vulnerable a inyección SQL.
**Evidencia:** El test falló con `x'` debido a una `SqliteException` en `ProductoRepository.BuscarPorNombre`.
**Causa raíz:** El término recibido del usuario se concatenaba directamente en la consulta SQL.
**Corrección:** Se eliminó el SQL concatenado y se utilizó LINQ con `EF.Functions.Like`.
**Prueba:** Se activó `TICK206_Buscador_NoPermiteInyeccionNiRompeConComilla`.
**Validación:** Se probaron entradas con comilla e intento de inyección SQL.
**Resultado:** La búsqueda ya no rompe con `x'`.
**Resultado:** El payload `' OR 1=1 --` no devuelve todo el catálogo.
**Tests:** `Superado: 5`, `Con error: 0`, `Omitido: 0`.
**Estado:** Resuelto y validado.

## TICK-301 corregir comportamiento inseguro del buscador
**Síntoma:** El buscador ejecutaba contenido HTML/JavaScript ingresado por el usuario.
**Evidencia:** El payload `<img src=x onerror=alert('XSS')>` ejecutó una alerta.
**Causa raíz:** Uso de `dangerouslySetInnerHTML` con datos controlados por el usuario.
**Corrección:** Se reemplazó por renderizado normal de JSX.
**Cambio:** El término y `p.Nombre` ahora se muestran como texto.
**Validación:** Se reconstruyó el frontend y se repitió el mismo payload.
**Resultado:** El contenido se mostró literalmente y no se ejecutó JavaScript.
**Impacto:** Se elimina el riesgo de XSS reflejado en este componente.
**Regresión:** La funcionalidad del buscador continúa operativa.
**Estado:** Resuelto y validado.

## TICK-302 — Diferencia entre monto estimado y cobrado

**Síntoma:** El panel mostraba un total diferente al cobro real.
**Evidencia:** Con Teclado Mecánico y 10% de descuento, el panel mostraba $51.40 y la API $50.75.
**Causa raíz:** El frontend calculaba el impuesto sobre el subtotal antes del descuento.
**Corrección:** Se calculó primero el descuento, luego la base imponible y finalmente el impuesto.
**Validación:** Se reconstruyó el frontend y se repitió el mismo escenario.
**Resultado:** El total estimado y el total cobrado ahora coinciden en $50.75.
**Regresión:** Se mantuvo el formato monetario a dos decimales.
**Estado:** Resuelto y validado.

## TICK-303 — Pedidos duplicados por doble clic

**Síntoma:** Una acción repetida podía generar pedidos duplicados.
**Evidencia:** La prueba inicial generó dos pedidos con pocos milisegundos de diferencia.
**Causa raíz:** El formulario no bloqueaba inmediatamente una segunda ejecución.
**Corrección:** Se agregó un bloqueo con `useRef` y se deshabilitó el botón durante el envío.
**Validación:** Con doble clic rápido se observó una sola petición `POST /api/pedidos` en Network.
**Resultado:** La segunda acción no genera otra solicitud mientras el primer envío está en proceso.
**Estado:** Resuelto y validado.

## TICK-304 — Reporte pesado en el panel

**Síntoma:** El reporte puede afectar el rendimiento del navegador al mostrar muchos registros.
**Evidencia:** El componente renderizaba todas las filas recibidas mediante `filas.map(...)`.
**Causa raíz:** No existía paginación en la presentación del reporte.
**Corrección:** Se agregó paginación de 50 registros por página.
**Validación:** El reporte continúa mostrando los 22 pedidos existentes y el total general de $1153.75.
**Resultado:** Con los datos actuales se muestran 22 de 22 registros en una sola página.
**Comportamiento esperado:** Con volúmenes mayores, el navegador renderiza como máximo 50 filas por página.
**Regresión:** No se modificó el cálculo ni los datos del reporte.
**Estado:** Resuelto y validado.

## TICK-305 — Manejo incorrecto de errores en pedidos

**Síntoma:** Al fallar un pedido, el panel no mostraba información clara al usuario.
**Evidencia:** Con stock insuficiente, la operación fallaba sin mensaje visible.
**Causa raíz:** El frontend no verificaba correctamente respuestas HTTP fallidas.
**Corrección:** Se validó `res.ok` y se agregó manejo de errores en el formulario.
**Validación:** Se probó un pedido con stock insuficiente.
**Resultado:** El pedido fallido ya no se muestra como exitoso y aparece un mensaje de error.
**Estado:** Resuelto y validado.

## TICK-306 — Rango de fechas excluye registros del día final

**Síntoma:** El reporte podía omitir pedidos realizados durante el día indicado en la fecha final.
**Causa raíz:** El límite superior se comparaba de forma inclusiva contra una fecha con hora 00:00.
**Corrección:** Se usa el límite superior exclusivo del día siguiente.
**Validación:** Se consultó desde `2026-08-27` hasta `2026-08-27`.
**Resultado:** El reporte incluyó los pedidos registrados durante todo el 27 de agosto.
**Regresión:** Se mantiene el filtrado por rango sin alterar los datos reportados.
**Estado:** Resuelto y validado.

## TICK-307 — Buscador dispara consultas mientras se escribe

**Síntoma:** Al escribir rápidamente en el buscador se generaban múltiples consultas y existía riesgo de mostrar resultados de búsquedas anteriores.
**Evidencia:** Antes de la corrección, Network mostró múltiples solicitudes al endpoint de búsqueda durante una misma interacción.
**Causa raíz:** El `useEffect` ejecutaba `buscarProductos()` en cada cambio del término, sin debounce ni control de respuestas obsoletas.
**Corrección:** Se agregó un debounce de 300 ms y una bandera de vigencia para ignorar respuestas de búsquedas anteriores.
**Validación:** Con `Slow 4G`, después de limpiar Network y escribir rápidamente `teclado`, se observó una única petición `buscar?termino=teclado`.
**Resultado:** Se redujeron las consultas innecesarias y solo la búsqueda vigente puede actualizar los resultados.
**Comportamiento esperado:** El panel debe esperar brevemente después de que el usuario deje de escribir y mostrar únicamente los resultados correspondientes al término actual.
**Regresión:** El buscador continúa mostrando correctamente los productos encontrados y conserva la búsqueda manual mediante Enter o botón.
**Estado:** Resuelto y validado.