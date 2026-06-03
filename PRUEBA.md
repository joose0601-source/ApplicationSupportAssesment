# Prueba técnica — Analista/Ingeniero de Soporte de Sistemas .NET

**Documento del candidato**

---

## Bienvenida y contexto

Te incorporas al equipo de **Soporte de Aplicaciones** que sostiene sistemas críticos en producción. En tu día a día atenderás incidentes reportados por usuarios de negocio, diagnosticarás fallas sobre soluciones que **no escribiste tú**, aplicarás correcciones de código con cuidado quirúrgico (sin romper lo que ya funciona), priorizarás múltiples incidentes a la vez y comunicarás con áreas no técnicas.

Esta prueba **no usa un sistema bancario** a propósito: queremos evaluar tu capacidad técnica y de razonamiento sobre una solución desconocida, no tu conocimiento previo del dominio. El sistema es **MercadoVerde**, una tienda en línea de accesorios tecnológicos construida en **ASP.NET Core 8 + Entity Framework Core + SQLite**.

> Si algo no funciona, ese suele ser el punto. Tu trabajo es entender *por qué*, no asumir.

## Reglas y entregables

- **Duración sugerida:** 3 horas (presencial guiada) o medio día (take-home). Avísanos si te falta tiempo; valoramos el *cómo* tanto como el *cuánto*.
- **Puedes** usar tu IDE, internet, documentación oficial y depurador. **Documenta** lo que consultes.
- **No puedes** reescribir la solución desde cero. Se evalúa tu capacidad de intervenir código existente.
- **Entrega:**
  1. El código modificado (un `.zip` o un branch de git con commits atómicos y mensajes claros).
  2. Un documento corto (`HALLAZGOS.md`) con tu bitácora: qué encontraste, dónde, por qué pasaba y cómo lo resolviste.
  3. Tu respuesta a las Partes 1 y 5 (pueden ir en el mismo documento).

Trabaja en voz alta si la prueba es presencial. Preferimos ver tu razonamiento aunque no termines todo.

---

## Parte 0 — Conoce el sistema (15 min)

Levanta la solución siguiendo el `README.md`. Explora el código y los endpoints en Swagger. Antes de tocar nada, responde brevemente:

1. ¿Cuál es el flujo completo cuando se crea un pedido (`POST /api/pedidos`)? Enuméralo en pasos.
2. ¿Qué servicios participan y de qué se encarga cada uno?
3. ¿Dónde vive la lógica de cobro, la de inventario y la de impuestos?

*(No buscamos perfección; queremos ver cómo abordas un código ajeno.)*

---

## Parte 1 — Triage y priorización (gestión y criterio) (20 min)

Es lunes 9:00 a.m. Tu bandeja amaneció con **seis tickets** (Anexo A). No puedes resolverlos todos a la vez.

Entrega una **tabla de priorización** con tu orden de atención. Para cada ticket indica:

- Severidad e impacto (a negocio, a clientes, a datos, a seguridad).
- Urgencia y por qué.
- Qué harías **primero** aunque aún no tengas la causa (mitigación o contención).
- A quién escalarías o informarías, si aplica.

No necesitas haber resuelto nada todavía. Queremos ver tu **criterio**.

---

## Parte 2 — Diagnóstico a partir de evidencia (25 min)

Tienes el archivo `logs-produccion.txt` y los tickets del Anexo A. **Sin corregir aún**, para **al menos cuatro** síntomas distintos escribe una hipótesis de causa raíz:

- ¿Qué archivo/método sospechas y por qué?
- ¿Qué evidencia del log o del ticket te lleva ahí?
- ¿Cómo lo **reproducirías** de forma controlada?

Pista de método: separa *síntoma* (lo que ve el usuario) de *causa* (lo que ocurre en el código). Un mismo síntoma puede tener varias causas candidatas; di cómo descartarías las que no son.

---

## Parte 3 — Corrección de defectos (lo central) (90 min)

Sobre el código, resuelve los incidentes que diagnosticaste. Hay **al menos siete defectos** de naturaleza distinta. Para cada corrección:

- Haz el cambio **mínimo y seguro** que resuelva la causa raíz (no parches que tapen el síntoma).
- Explica en `HALLAZGOS.md` el antes/después y **por qué** tu solución es correcta.
- Cuida no introducir regresiones. Si un cambio tiene efectos colaterales, decláralos.

Criterios que valoramos especialmente:
- Integridad de datos y comportamiento bajo **concurrencia**.
- **Seguridad** (entradas no confiables).
- **Corrección de la lógica de negocio** (el dinero debe cuadrar al centavo).
- **Observabilidad**: que una falla nunca quede invisible.
- **Rendimiento** sobre volúmenes reales.

> No te diremos cuántos defectos hay en cada archivo. Encontrar lo que nadie te señaló es parte de la evaluación.

---

## Parte 4 — Cambio de requerimiento (implementación) (30 min)

Negocio solicita una mejora pequeña pero real:

> *"Queremos un tope: ningún cupón puede generar un descuento mayor a **$15.00** en un pedido, sin importar el porcentaje. Si el descuento calculado lo supera, se aplica $15.00 y se deja constancia."*

Impleméntalo respetando el estilo del código existente. Considera: ¿dónde va esta regla?, ¿cómo la harías testeable?, ¿cómo dejarías "constancia" de forma útil para soporte?

*(Si te alcanza el tiempo, propón cómo lo cubrirías con una prueba automatizada, aunque no la escribas completa.)*

---

## Parte 5 — Comunicación y cierre (gestión) (20 min)

Elige **uno** de los incidentes que corregiste y redacta:

1. Un **RCA (análisis de causa raíz) breve** para el equipo técnico: causa, impacto, corrección, y **una acción preventiva** para que no se repita.
2. Un **mensaje al área de negocio** (3–5 líneas, sin jerga técnica) explicando qué pasó, qué se hizo y qué deben esperar ahora.

---

## Anexo A — Bandeja de incidentes

**TICK-201 — "Vendimos un monitor que no teníamos"**
Logística reporta que se confirmaron **dos pedidos del Monitor 24"** casi al mismo tiempo y el sistema dejó el stock en **−1**. Pasa cuando hay campañas y entra mucho tráfico junto. Prioridad de negocio: alta, afecta cumplimiento de entregas.

**TICK-202 — "El cobro con cupón no cuadra"**
Finanzas reporta que en pedidos con un cupón **válido** (ej. `BIENVENIDA10`) *"el impuesto sale más caro de lo que debería"* y el total final no coincide con su hoja de cálculo. Además, soporte sospecha que **la vigencia de los cupones se comporta distinto según la hora del día** (un cupón parece seguir válido o caer fuera de fecha sin explicación clara).

**TICK-203 — "Algunos pedidos truenan con error 500"**
Atención al cliente reporta pantallas de error al confirmar ciertos pedidos. No es siempre; el patrón parece ser: ocurre con **un cliente en particular** y también al usar el **código de campaña `PROMO50`** que Marketing publicó. *Nota: el reporte verbal de los usuarios es impreciso; confía en la evidencia.*

**TICK-204 — "El reporte de ventas se cuelga"**
Finanzas dice que el **reporte de ventas** tarda casi un minuto y a veces **expira** antes de cargar. Empezó cuando creció el volumen de pedidos.

**TICK-205 — "Un pedido quedó 'Pagado' pero el dinero nunca entró"**
Conciliación encontró un pedido en estado **Pagado** que **no tiene referencia de la pasarela**. Sospechan que el cobro falló y el sistema igual lo dio por bueno. Riesgo financiero.

**TICK-206 — "El buscador hace cosas raras"**
Un usuario reportó que al escribir ciertos caracteres en el **buscador de productos** el sistema devuelve resultados extraños o errores. Seguridad pide que lo revises **con prioridad**.

---

*Fin del documento del candidato.*
