"use client";

import { useState } from "react";

import { obtenerReporteVentas } from "@/lib/api";
import { formatearMoneda } from "@/lib/money";
import type { FilaReporte } from "@/lib/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export function ReporteVentas() {
  const [desde, setDesde] = useState("2026-01-01");
  const [hasta, setHasta] = useState("2026-06-02");
  const [filas, setFilas] = useState<FilaReporte[]>([]);
  const [cargando, setCargando] = useState(false);
  const [pagina, setPagina] = useState(1);

const FILAS_POR_PAGINA = 50;

  async function generar() {
    setCargando(true);
    const data = await obtenerReporteVentas(desde, hasta);
  setFilas(data);
setPagina(1);
    setCargando(false);
  }

  // Total general de todas las ventas del rango.
  const totalGeneral = filas.reduce((acc, f) => acc + f.Total, 0);
const totalPaginas = Math.ceil(filas.length / FILAS_POR_PAGINA);

const filasPagina = filas.slice(
  (pagina - 1) * FILAS_POR_PAGINA,
  pagina * FILAS_POR_PAGINA
);
  return (
    <Card>
      <CardHeader>
        <CardTitle>Reporte de ventas</CardTitle>
        <CardDescription>
          Ventas por rango de fechas. En producción la tabla de pedidos tiene
          cientos de miles de filas.
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-6">
        <div className="flex flex-wrap items-end gap-3">
          <div className="space-y-2">
            <Label htmlFor="desde">Desde</Label>
            <Input
              id="desde"
              type="date"
              value={desde}
              onChange={(e) => setDesde(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="hasta">Hasta</Label>
            <Input
              id="hasta"
              type="date"
              value={hasta}
              onChange={(e) => setHasta(e.target.value)}
            />
          </div>
          <Button onClick={generar} disabled={cargando}>
            {cargando ? "Generando…" : "Generar reporte"}
          </Button>
        </div>

        {filas.length > 0 && (
          <>
            <div className="flex items-center justify-between rounded-lg border bg-muted/40 p-4">
              <span className="text-sm text-muted-foreground">
                {filas.length} pedidos en el rango
              </span>
              <span className="text-lg font-semibold">
                Total: {formatearMoneda(totalGeneral)}
              </span>
            </div>

            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Pedido</TableHead>
                  <TableHead>Cliente</TableHead>
                  <TableHead className="text-right">Artículos</TableHead>
                  <TableHead className="text-right">Total</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filasPagina.map((fila) => (
                     <TableRow key={fila.PedidoId}>
                      <TableCell className="font-medium">{fila.PedidoId}</TableCell>
                    <TableCell>{fila.Cliente}</TableCell>
                    <TableCell className="text-right">
                      {fila.CantidadArticulos}
                    </TableCell>
                    <TableCell className="text-right">
                      {formatearMoneda(fila.Total)}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
            <div className="flex items-center justify-between">
  <span className="text-sm text-muted-foreground">
    Mostrando {filasPagina.length} de {filas.length} pedidos
  </span>

  <div className="flex gap-2">
    <Button
      variant="outline"
      size="sm"
      onClick={() => setPagina((p) => p - 1)}
      disabled={pagina === 1}
    >
      Anterior
    </Button>

    <span className="flex items-center px-2 text-sm">
      Página {pagina} de {totalPaginas}
    </span>

    <Button
      variant="outline"
      size="sm"
      onClick={() => setPagina((p) => p + 1)}
      disabled={pagina === totalPaginas}
    >
      Siguiente
    </Button>
  </div>
</div>
          </>
        )}
      </CardContent>
    </Card>
  );
}
