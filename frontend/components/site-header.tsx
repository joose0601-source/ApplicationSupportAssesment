import Link from "next/link";
import { Leaf } from "lucide-react";

const navItems = [
  { href: "/", label: "Catálogo" },
  { href: "/pedidos", label: "Crear pedido" },
  { href: "/reportes", label: "Reporte de ventas" },
];

export function SiteHeader() {
  return (
    <header className="sticky top-0 z-40 w-full border-b bg-background/95 backdrop-blur">
      <div className="container flex h-16 items-center justify-between">
        <Link href="/" className="flex items-center gap-2 font-bold">
          <Leaf className="h-5 w-5 text-primary" />
          <span>MercadoVerde</span>
          <span className="text-xs font-normal text-muted-foreground">
            · Panel de soporte
          </span>
        </Link>
        <nav className="flex items-center gap-1">
          {navItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className="rounded-md px-3 py-2 text-sm font-medium text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
            >
              {item.label}
            </Link>
          ))}
        </nav>
      </div>
    </header>
  );
}
