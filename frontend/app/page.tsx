import { BuscadorProductos } from "@/components/buscador-productos";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

export default function HomePage() {
  return (
    <div className="space-y-8">
      <Card className="border-primary/20 bg-primary/5">
        <CardHeader>
          <CardTitle>Panel de soporte · MercadoVerde</CardTitle>
          <CardDescription>
            Herramienta interna del equipo de soporte para operar el catálogo,
            registrar pedidos y consultar reportes contra la API de la tienda.
            Asegúrate de tener la API .NET corriendo (ver el README de la raíz).
          </CardDescription>
        </CardHeader>
        <CardContent className="grid gap-4 text-sm text-muted-foreground sm:grid-cols-3">
          <div>
            <p className="font-medium text-foreground">Catálogo</p>
            <p>Busca productos por nombre y revisa stock.</p>
          </div>
          <div>
            <p className="font-medium text-foreground">Pedidos</p>
            <p>Registra pedidos con cupones e impuestos.</p>
          </div>
          <div>
            <p className="font-medium text-foreground">Reportes</p>
            <p>Ventas por rango de fechas.</p>
          </div>
        </CardContent>
      </Card>

      <BuscadorProductos />
    </div>
  );
}
