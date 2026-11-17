import { Link } from "@tanstack/react-router";
import { type Column, type ColumnDef } from "@tanstack/react-table";
import { ArrowDownIcon, ArrowUpIcon, CaretSortIcon } from "@radix-ui/react-icons";
import { MapPin } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import type { Order } from "../../../common/schemas";

const dateFormatter = new Intl.DateTimeFormat("sv-SE", {
  dateStyle: "medium",
});

function sortableHeader<TData, TValue>({
  column,
  title,
}: {
  column: Column<TData, TValue>;
  title: string;
}) {
  const sorted = column.getIsSorted();

  return (
    <Button
      type="button"
      variant="ghost"
      size="sm"
      className="h-8 px-0 has-[>svg]:px-0"
      aria-label={`Sort ${title} ${sorted === "asc" ? "descending" : "ascending"}`}
      onClick={() => column.toggleSorting(sorted === "asc")}
    >
      <span>{title}</span>
      {sorted === "asc" ? (
        <ArrowUpIcon className="ms-2 h-4 w-4" />
      ) : sorted === "desc" ? (
        <ArrowDownIcon className="ms-2 h-4 w-4" />
      ) : (
        <CaretSortIcon className="ms-2 h-4 w-4" />
      )}
    </Button>
  );
}

export const logisticsOrdersColumns: ColumnDef<Order>[] = [
  {
    accessorKey: "orderNumber",
    header: ({ column }) => sortableHeader({ column, title: "Order" }),
    cell: ({ row }) => (
      <Link
        to="/orders/$orderId"
        params={{ orderId: String(row.original.id) }}
        className="font-medium hover:underline"
      >
        {row.original.number}
      </Link>
    ),
    enableSorting: true,
  },
  {
    id: "destination",
    accessorFn: (order) => order.destinationLocation.cityName,
    header: ({ column }) =>
      sortableHeader({ column, title: "Destination" }),
    cell: ({ row }) => {
      const destination = row.original.destinationLocation;
      return (
        <div className="flex items-start gap-2">
          <MapPin
            className="mt-0.5 size-4 shrink-0 text-muted-foreground"
            aria-hidden="true"
          />
          <span>
            <span className="block font-medium">{destination.cityName}</span>
            <span className="text-xs text-muted-foreground">
              {destination.countryName}, {destination.countryCode}
            </span>
          </span>
        </div>
      );
    },
    enableSorting: true,
  },
  {
    accessorKey: "status",
    header: ({ column }) => sortableHeader({ column, title: "Status" }),
    cell: ({ row }) => {
      const status =
        row.original.transportOrder?.transportOrderStatus ?? "Unknown";
      const normalizedStatus = status.toLowerCase();
      return (
        <Badge
          variant={
            normalizedStatus === "cancelled" ? "destructive" : "secondary"
          }
          className={
            normalizedStatus === "accepted"
              ? "border-transparent bg-green-100 text-green-800 hover:bg-green-100 dark:bg-green-900/30 dark:text-green-300"
              : normalizedStatus === "requested"
                ? "border-gray-200 bg-white text-gray-900 hover:bg-gray-50 dark:bg-white dark:text-gray-900 dark:hover:bg-gray-100"
                : undefined
          }
        >
          {status}
        </Badge>
      );
    },
    enableSorting: true,
  },
  {
    accessorKey: "estimatedDelivery",
    header: ({ column }) =>
      sortableHeader({ column, title: "Estimated Delivery" }),
    cell: ({ row }) =>
      row.original.estimatedDelivery
        ? dateFormatter.format(new Date(row.original.estimatedDelivery))
        : "",
    enableSorting: true,
  },
  {
    accessorKey: "placedAt",
    header: ({ column }) =>
      sortableHeader({ column, title: "Placed At" }),
    cell: ({ row }) =>
      row.original.placedAt
        ? dateFormatter.format(new Date(row.original.placedAt))
        : "",
    enableSorting: true,
  }
];
