# Prueba técnica — Analista/Ingeniero de Soporte de Sistemas .NET

**Documento del candidato**

---

## Bienvenida y contexto

Te incorporas al equipo de **Soporte de Aplicaciones** que sostiene sistemas críticos en producción. En tu día a día atenderás incidentes reportados por usuarios de negocio, diagnosticarás fallas sobre soluciones que **no escribiste tú**, aplicarás correcciones de código con cuidado quirúrgico (sin romper lo que ya funciona), priorizarás múltiples incidentes a la vez y comunicarás con áreas no técnicas.

Esta prueba **no usa un sistema bancario** a propósito: queremos evaluar tu capacidad técnica y de razonamiento sobre una solución desconocida, no tu conocimiento previo del dominio. El sistema es **MercadoVerde**, una tienda en línea de accesorios tecnológicos construida en **ASP.NET Core 8 + Entity Framework Core + PostgreSQL**, con un panel de soporte en **Next.js**. Todo corre en **contenedores** con Docker Compose (`docker compose up --build`).

> Si algo no funciona, ese suele ser el punto. Tu trabajo es entender *por qué*, no asumir.

## Antes de empezar

1. **Levanta el sistema** siguiendo el [`README.md`](./README.md): `docker compose up --build` arranca base de datos, API y panel. Verifica la "línea base" (que todo abre y responde) **antes** de tocar nada.
2. **Lee este documento completo.** Son 6 partes; cada una indica qué entregar y cuánto tiempo invertir.
3. **Revisa la evidencia:** los tickets (Anexos A y B) y los logs en [`logs-produccion.txt`](./logs-produccion.txt).
4. **Crea tu rama:** `git checkout -b solucion-<tu-nombre>`.

## Reglas

- **Duración sugerida:** ~3 h para el backend (Partes 0–5). La **Parte 6 (frontend)** suma ~1 h y es **opcional según el rol** — confírmalo con quien te asignó la prueba. Avísanos si te falta tiempo; valoramos el *cómo* tanto como el *cuánto*.
- **Puedes** usar tu IDE, internet, documentación oficial, herramientas de IA y depurador. **Documenta** lo que consultes y por qué.
- **No puedes** reescribir la solución desde cero ni cambiar el stack. Se evalúa tu capacidad de **intervenir código ajeno** con cambios mínimos y seguros.
- **No te diremos cuántos defectos hay ni dónde.** Encontrar lo que nadie te señaló es parte de la evaluación.
- **Puntos extra:** más allá de los incidentes listados, el sistema tiene otros defectos y oportunidades de *hardening* (seguridad, concurrencia, validación, calidad de API) que **no** están en ningún ticket. Detectarlos y corregirlos bien —o al menos dejarlos documentados en `HALLAZGOS.md`— **suma**.
- Si la prueba es presencial, **trabaja en voz alta**: preferimos ver tu razonamiento aunque no termines todo.

## Qué entregar

1. **Una rama de git** `solucion-<tu-nombre>` con **commits atómicos** (idealmente uno por incidente) y mensajes claros que referencien el ticket (ej. `TICK-203: valida cliente sin email antes de generar comprobante`). Entrégala como Pull Request a este repositorio (o como `.zip` si así se acordó).
2. **`HALLAZGOS.md`** — tu bitácora (usa la plantilla de abajo). Como mínimo debe incluir:
   - Tus respuestas a la **Parte 0**, la tabla de la **Parte 1**, las hipótesis de la **Parte 2** y la comunicación de la **Parte 5**.
   - Por cada defecto corregido: **síntoma → causa raíz → archivo → corrección → posible regresión**.
3. Si haces la **Parte 6**, añade una sección **"Frontend"** dentro del mismo `HALLAZGOS.md`.

## Cómo trabajar (recomendado)

Para cada incidente, repite este ciclo corto:

1. **Reproduce** el síntoma de forma controlada (Swagger, `curl` o el panel). Si no lo reproduces, todavía no lo entiendes.
2. **Aísla la causa raíz** en el código. Separa *síntoma* (lo que ve el usuario) de *causa* (lo que ocurre en el código).
3. **Corrige con el cambio mínimo** que ataque la causa, no el síntoma. No refactorices de más.
4. **Verifica** que el síntoma desaparece y que no rompiste nada que antes funcionaba.
5. **Documenta y commitea** (un commit por incidente, con el número de ticket en el mensaje).

Prioriza por riesgo: **seguridad** e **integridad de datos/dinero** antes que lo cosmético. No necesitas terminar todo; ordena bien y explica tu criterio.

## Plantilla sugerida para `HALLAZGOS.md`

````markdown
# HALLAZGOS — <tu nombre>

## Parte 0 — Conozco el sistema
- Flujo de `POST /api/pedidos`: ...
- Servicios y responsabilidades: ...

## Parte 1 — Triage y priorización
| Orden | Ticket | Severidad / Impacto | Urgencia | Contención inmediata | Escalo a |
|-------|--------|---------------------|----------|----------------------|----------|
| 1 | TICK-xxx | ... | ... | ... | ... |

## Parte 2 — Hipótesis de causa raíz
### TICK-xxx — <título>
- **Sospecho:** `<archivo/método>` porque ...
- **Evidencia:** `<línea del log / dato del ticket>`
- **Reproducción:** <pasos>

## Parte 3 (y 6) — Correcciones
### TICK-xxx — <título> · commit `<hash>`
- **Síntoma:** ...
- **Causa raíz:** `<archivo:línea>` — ...
- **Corrección:** antes/después y por qué es correcta.
- **Regresiones / efectos colaterales:** ...

## Parte 4 — Tope de descuento
- Dónde lo puse y por qué: ...
- Cómo lo probaría: ...

## Parte 5 — Comunicación
- **RCA (equipo técnico):** ...
- **Mensaje a negocio:** ...
````

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

## Parte 6 — Soporte de frontend (opcional / si aplica al rol) (60 min)

El equipo de soporte también mantiene el **panel interno** (`frontend/`), una app
**Next.js + shadcn/ui** que el área de atención usa para buscar productos,
registrar pedidos y consultar reportes contra la misma API. Negocio levantó una
**segunda bandeja de incidentes** sobre ese panel (Anexo B).

Aplica el **mismo método** que en las Partes 2 y 3, ahora sobre el frontend:

1. **Levanta el panel** siguiendo `frontend/README.md` (necesitas la API corriendo).
2. **Reproduce** cada síntoma del Anexo B en el navegador.
3. **Diagnostica** la causa raíz (archivo/componente y por qué pasa). Distingue
   los bugs que viven en el **frontend** de los que en realidad son **del backend**
   y solo se *ven* en la interfaz.
4. **Corrige** con el cambio mínimo y seguro, sin romper lo que funciona.
5. Documenta el antes/después en `HALLAZGOS.md` (puedes usar una sección
   "Frontend").

Hay **bastantes defectos** de frontend (más de una docena) de naturaleza y
dificultad distintas — desde detalles mecánicos hasta problemas sutiles de
concurrencia y rendimiento. Valoramos especialmente:

- **Seguridad del lado del cliente** (entradas no confiables, XSS, higiene de enlaces).
- **Integridad de las acciones del usuario** (evitar acciones duplicadas / dobles cobros).
- **Validación de entradas** en la interfaz (cantidades, rangos, datos inválidos).
- **Corrección y formato del dinero** en la interfaz (debe cuadrar al centavo).
- **Observabilidad de errores** (una falla de red o un 500 nunca deben quedar invisibles para el agente).
- **Rendimiento** sobre volúmenes reales (cientos de miles de filas) y **concurrencia en la interfaz** (peticiones que se pisan).
- **Manejo correcto de fechas/zonas horarias** y de listas/estado en React.

> Igual que en el backend: no te decimos cuántos defectos hay en cada archivo.
> Encontrar lo que nadie te señaló es parte de la evaluación. Varios defectos
> (**accesibilidad**, **buenas prácticas de React**, **validación** y **seguridad
> básica**) no están en ningún ticket: se ven leyendo el código y usando el panel.

---

## Anexo A — Bandeja de incidentes (backend)

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

## Anexo B — Bandeja de incidentes (frontend / panel de soporte)

> Estos tickets son sobre el **panel interno** (`frontend/`). Reprodúcelos en el
> navegador con la API corriendo.

**TICK-301 — "El buscador del panel ejecuta cosas"**
Seguridad hizo una prueba: al buscar un texto con etiquetas HTML (por ejemplo
`<img src=x onerror=alert(1)>`) o al existir un producto con un nombre así, el
panel **renderiza/ejecuta** ese contenido en lugar de mostrarlo como texto.
Pide revisión **con prioridad**: es la versión de cara al cliente del mismo
problema que ya viste en el catálogo.

**TICK-302 — "Los montos del panel se ven raros y no cuadran"**
Atención al cliente reporta que en la pantalla de **crear pedido**, el subtotal
de algunas líneas aparece con **muchos decimales** (ej. `149.70000000000002`) y
**sin formato de moneda**. Finanzas además dice que el **"total estimado"** que
muestra el panel **no siempre coincide** con el total que termina cobrando la
API. Quieren que el panel sea confiable para confirmar montos con el cliente.

**TICK-303 — "Se crearon dos pedidos iguales"**
Un agente reporta que al confirmar un pedido, si el botón **tarda** y vuelve a
hacer clic (o lo hace dos veces rápido), el sistema **registra el pedido dos
veces** y descuenta stock de más. Logística lo está sufriendo en campañas.

**TICK-304 — "El reporte deja el navegador pegado"**
Finanzas dice que al generar el **reporte de ventas** de un rango grande, la
pestaña **se congela** y a veces el navegador pide "esperar o cerrar la página".
Empezó cuando creció el volumen de pedidos.

**TICK-305 — "Cuando la API falla, el panel no dice nada"**
Soporte reporta que si la API está caída o devuelve error, el buscador y los
reportes simplemente muestran **"0 resultados"** o una pantalla en blanco, sin
ningún mensaje de error. El agente cree que "no hay datos" cuando en realidad
**la llamada falló**. Lo mismo al crear un pedido que truena en el backend.

**TICK-306 — "El reporte no incluye los pedidos del último día"**
Finanzas nota que al pedir el reporte hasta una fecha (ej. `hasta = hoy`), los
pedidos **de ese mismo día no aparecen**; tienen que poner la fecha del día
siguiente para verlos. Sospechan algo con cómo el panel arma el rango de fechas.

**TICK-307 — "El buscador se comporta raro al escribir rápido"**
Atención al cliente reporta que, al teclear rápido en el buscador, a veces
**aparecen resultados de una búsqueda anterior** (no los del texto que quedó en
la caja), y que el panel **dispara muchísimas consultas** a la API mientras se
escribe. Sospechan que se "pisan" las respuestas.

**TICK-308 — "El panel dice 'en stock' aunque casi no queda"**
Logística reporta que el catálogo del panel muestra el indicador **verde "en
stock"** incluso cuando quedan **1 o 2 unidades**, y eso los llevó a prometer
inventario que no había. Piden que el panel **avise cuando el stock está bajo**.

**TICK-309 — "Se cuelan pedidos con cantidades inválidas"**
Soporte reporta que en **crear pedido** se pueden capturar cantidades en **0 o
negativas** (o dejar el campo vacío) y el panel **igual deja confirmar**; el
total estimado sale en cifras sin sentido. Quieren que no se permita.

---

*Fin del documento del candidato.*
