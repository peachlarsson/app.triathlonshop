import { getRouteApi, Link } from "@tanstack/react-router";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { useOrder } from "./hooks/use-orders";

const route = getRouteApi("/orders/$orderId");

const currencyFormatter = new Intl.NumberFormat("sv-SE", {
  style: "currency",
  currency: "SEK",
});

const dateFormatter = new Intl.DateTimeFormat("sv-SE", {
  dateStyle: "medium",
  timeStyle: "short",
});

export default function OrderDetails() {
  const { orderId } = route.useParams();

  const { data: order, isPending, error } = useOrder(orderId);

  if (isPending) return <p>Loading order...</p>;
  if (error) return <p role="alert">{error.message}</p>;
  if (!order) return <p>Order not found.</p>;

  return (
    <>
      <Header />

      <Main className="flex flex-1 flex-col gap-4 sm:gap-6">
        <div className="flex flex-wrap items-start justify-between gap-3">
          <div>
            <h2 className="text-2xl font-bold tracking-tight">Order {order.number}</h2>
            <p className="text-muted-foreground">Order ID: {order.id}</p>
            <p className="text-muted-foreground">Placed {dateFormatter.format(new Date(order.createdAt))}</p>
          </div>
          <div className="flex items-center gap-2">
            <Badge>{order.status}</Badge>
            <Button asChild variant="outline" size="sm">
              <Link to="/orders">Back to orders</Link>
            </Button>
          </div>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Order items</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-hidden rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Product</TableHead>
                    <TableHead className="text-right">Unit price</TableHead>
                    <TableHead className="text-right">Qty</TableHead>
                    <TableHead className="text-right">Line total</TableHead>
                    <TableHead className="text-right">Created at</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {order.orderRows.map((item) => (
                    <TableRow key={item.id}>
                      <TableCell className="font-medium">{item.articleName}</TableCell>
                      <TableCell className="text-right">{currencyFormatter.format(item.unitPrice)}</TableCell>
                      <TableCell className="text-right">{item.quantity}</TableCell>
                      <TableCell className="text-right">
                        {currencyFormatter.format(item.rowPrice)}
                      </TableCell>
                      <TableCell className="text-right">{dateFormatter.format(new Date(item.createdAt))}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>

        <div className="grid gap-4 lg:grid-cols-3">
          <Card className="lg:col-span-2">
            <CardHeader>
              <CardTitle>Payment summary</CardTitle>
            </CardHeader>
            <CardContent className="space-y-2 text-sm">
              <div className="flex items-center justify-between">
                <span className="text-muted-foreground">Subtotal</span>
                <span>{currencyFormatter.format(order.totalPrice)}</span>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-muted-foreground">Shipping</span>
                <span></span>
              </div>
              <Separator />
              <div className="flex items-center justify-between font-medium">
                <span>Total</span>
                <span>{currencyFormatter.format(order.totalPrice)}</span>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Customer</CardTitle>
            </CardHeader>
            <CardContent className="space-y-1 text-sm">
              <p className="font-medium">Name: {order.customer.name}</p>
              <p className="text-muted-foreground">Email: {order.customer.email}</p>
            </CardContent>
          </Card>
        </div>

      </Main>
    </>
  );
}
