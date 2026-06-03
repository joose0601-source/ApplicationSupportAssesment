# Prueba técnica — Soporte de Sistemas .NET · MercadoVerde

Bienvenido/a. Este repositorio contiene una **aplicación real con problemas reales**. Tu objetivo es comportarte como lo harías en un equipo de soporte de aplicaciones: entender un sistema que no escribiste, diagnosticar fallas a partir de evidencia, corregirlas con cuidado y comunicar lo que hiciste.

> Es un sistema de e-commerce (no bancario) a propósito: queremos ver tu capacidad técnica y de razonamiento, no tu conocimiento previo de un dominio. **Si algo no funciona, ese suele ser el punto.**

## 📋 Antes de empezar

1. Lee **[`PRUEBA.md`](./PRUEBA.md)** completo. Ahí están las 6 partes del ejercicio, los tiempos sugeridos y lo que debes entregar.
2. Revisa la bandeja de incidentes (Anexo A de `PRUEBA.md`) y los logs en **[`logs-produccion.txt`](./logs-produccion.txt)**.

## 🧱 El sistema

API REST de una tienda en línea de accesorios tecnológicos.

- **Stack:** ASP.NET Core 8 · Entity Framework Core · SQLite
- **Módulos:** catálogo, pedidos (con cupones e impuestos), cobro vía pasarela externa simulada, y reportes de ventas.

### Cómo ejecutarlo
```bash
cd src/MercadoVerde.Api
dotnet restore
dotnet run
```
La API levanta en `https://localhost:<puerto>/swagger`. La base `tienda.db` se crea y se
siembra automáticamente al iniciar; bórrala para reiniciar los datos.

| Método | Ruta | Descripción |
|---|---|---|
| GET  | `/api/productos/buscar?termino=mouse` | Busca productos por nombre |
| POST | `/api/pedidos` | Crea un pedido (cobra y descuenta stock) |
| GET  | `/api/reportes/ventas?desde=...&hasta=...` | Reporte de ventas por rango |

Cuerpo de ejemplo para `POST /api/pedidos`:
```json
{ "clienteId": 1, "codigoCupon": "BIENVENIDA10", "lineas": [ { "productoId": 2, "cantidad": 1 } ] }
```

## 📦 Qué entregar

1. **Tu código corregido.** Trabaja en una rama (`git checkout -b solucion-<tu-nombre>`) con **commits pequeños y mensajes claros** (uno por incidente, idealmente).
2. **`HALLAZGOS.md`** — tu bitácora: qué encontraste, en qué archivo, por qué pasaba y cómo lo resolviste.
3. **Tus respuestas a las Partes 1, 2 y 5** (pueden ir dentro de `HALLAZGOS.md`).

Entrega como Pull Request hacia este repositorio (o como `.zip` si así se acordó).

## ✅ Qué evaluamos

Diagnóstico y causa raíz · corrección de código y dominio de .NET/EF · pensamiento crítico · calidad y **seguridad** · gestión y priorización de incidentes · comunicación. No buscamos que lo termines todo: valoramos **cómo** razonas y trabajas tanto como el resultado.

## 📁 Mapa de archivos clave
- `src/MercadoVerde.Api/Services/PedidoService.cs` — cálculo, cobro y descuento de stock.
- `src/MercadoVerde.Api/Services/InventarioService.cs` — descuento de stock.
- `src/MercadoVerde.Api/Services/ReporteService.cs` — reporte de ventas.
- `src/MercadoVerde.Api/Data/ProductoRepository.cs` — búsqueda de catálogo.
- `src/MercadoVerde.Api/Models/Modelos.cs` — entidades de dominio.

¡Éxitos! Trabaja con calma y documenta tu razonamiento.
