import { Link } from "@tanstack/react-router";
import { ArrowLeft, ArrowRight, MapPin, Package } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { type Order } from "../../orders/data/schema";

const dateFormatter = new Intl.DateTimeFormat("sv-SE", {
  dateStyle: "medium",
});

type OrderDestinationsTableProps = {
  orders: Order[];
  page: number;
  pageCount: number;
  pageSize: number;
  totalCount: number;
  isLoading: boolean;
  error?: string;
  onPageChange: (page: number) => void;
};

export function OrderDestinationsTable({
  orders,
  page,
  pageCount,
  pageSize,
  totalCount,
  isLoading,
  error,
  onPageChange,
}: OrderDestinationsTableProps) {
  return (
    <section className="overflow-hidden rounded-lg border bg-card">
      <div className="flex items-center gap-2 border-b px-4 py-3">
        <Package className="size-4 text-muted-foreground" aria-hidden="true" />
        <h3 className="font-semibold">Order destinations</h3>
      </div>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Order</TableHead>
            <TableHead>Destination</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Date</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {orders.map((order) => (
            <TableRow key={order.id}>
              <TableCell>
                <Link
                  to="/orders/$orderId"
                  params={{ orderId: String(order.id) }}
                  className="font-medium hover:underline"
                >
                  {order.number}
                </Link>
              </TableCell>
              <TableCell>
                <div className="flex items-start gap-2">
                  <MapPin
                    className="mt-0.5 size-4 shrink-0 text-muted-foreground"
                    aria-hidden="true"
                  />
                  <span>
                    <span className="block font-medium">
                      {order.destinationLocation.city},{" "}
                      {order.destinationLocation.countryName}
                    </span>
                    <span className="text-xs text-muted-foreground">
                      {order.destinationLocation.countryCode}
                    </span>
                  </span>
                </div>
              </TableCell>
              <TableCell>
                <Badge
                  variant={
                    order.status.toLowerCase() === "cancelled"
                      ? "destructive"
                      : "secondary"
                  }
                >
                  {order.status}
                </Badge>
              </TableCell>
              <TableCell>
                {dateFormatter.format(new Date(order.createdAt))}
              </TableCell>
            </TableRow>
          ))}
          {error && (
            <TableRow>
              <TableCell
                colSpan={4}
                className="h-24 text-center text-destructive"
                role="alert"
              >
                {error}
              </TableCell>
            </TableRow>
          )}
          {isLoading && (
            <TableRow>
              <TableCell
                colSpan={4}
                className="h-24 text-center text-muted-foreground"
              >
                Loading orders…
              </TableCell>
            </TableRow>
          )}
          {!isLoading && !error && !orders.length && (
            <TableRow>
              <TableCell
                colSpan={4}
                className="h-24 text-center text-muted-foreground"
              >
                No orders found.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
      <div className="flex items-center justify-between border-t px-4 py-3">
        <p className="text-sm text-muted-foreground">
          {error
            ? "Order count unavailable"
            : totalCount
              ? `Showing ${(page - 1) * pageSize + 1}–${Math.min(page * pageSize, totalCount)} of ${totalCount} orders`
              : "No orders"}
        </p>
        <div className="flex gap-2">
          <Button
            variant="outline"
            size="sm"
            disabled={page <= 1 || isLoading}
            onClick={() => onPageChange(page - 1)}
          >
            <ArrowLeft aria-hidden="true" />
            Previous
          </Button>
          <Button
            variant="outline"
            size="sm"
            disabled={page >= pageCount || isLoading}
            onClick={() => onPageChange(page + 1)}
          >
            Next
            <ArrowRight aria-hidden="true" />
          </Button>
        </div>
      </div>
    </section>
  );
}
