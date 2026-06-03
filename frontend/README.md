# MercadoVerde — Panel de soporte (Frontend)

Frontend del panel interno de soporte de **MercadoVerde**, construido con
**Next.js 14 (App Router)**, **TypeScript**, **Tailwind CSS** y componentes
**shadcn/ui**. Consume la API de `src/MercadoVerde.WebApi`.

> Como el backend, **esta aplicación tiene problemas reales a propósito**. Forma
> parte de la prueba: además de los incidentes de backend, hay una bandeja de
> incidentes de **frontend** (ver `PRUEBA.md`, Parte 6 y Anexo B).

## Stack

- Next.js 14 · React 18 · TypeScript
- Tailwind CSS + shadcn/ui (Button, Input, Card, Table, Badge, etc.)
- `fetch` nativo contra la API REST

## Cómo ejecutarlo

**Con contenedores (recomendado).** Desde la raíz del repo, `docker compose up --build`
levanta base de datos, API y este panel juntos. El panel queda en
`http://localhost:3000` y la API en `http://localhost:5080`. La URL de la API se
inyecta en build vía el argumento `NEXT_PUBLIC_API_BASE` del servicio `web` (ver
`docker-compose.yml` y el `Dockerfile` de esta carpeta).

**Sin contenedores (opcional, para desarrollo del panel).**

```bash
cd frontend
npm install
cp .env.local.example .env.local   # NEXT_PUBLIC_API_BASE → http://localhost:5080
npm run dev
```

Abre `http://localhost:3000`. Necesita la API corriendo y accesible en la URL de
`NEXT_PUBLIC_API_BASE`.

> Las variables `NEXT_PUBLIC_*` se **embeben en tiempo de build**: en contenedor
> se fijan con el build arg; en `npm run dev` se leen de `.env.local`.

## Mapa de pantallas

| Ruta | Pantalla | Endpoint que consume |
|---|---|---|
| `/` | Catálogo / buscador de productos | `GET /api/productos/buscar` |
| `/pedidos` | Crear pedido (cupón, líneas, cobro) | `POST /api/pedidos` |
| `/reportes` | Reporte de ventas por rango | `GET /api/reportes/ventas` |

## Mapa de archivos clave

- `lib/api.ts` — cliente HTTP hacia la API.
- `lib/money.ts` — cálculo y formato de montos para el panel.
- `components/buscador-productos.tsx` — buscador del catálogo.
- `components/crear-pedido-form.tsx` — alta de pedidos y cobro.
- `components/reporte-ventas.tsx` — reporte de ventas.
- `components/ui/*` — primitivos de shadcn/ui.
