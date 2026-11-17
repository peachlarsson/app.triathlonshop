import { Link } from "@tanstack/react-router";
import { type ColumnDef } from "@tanstack/react-table";
import { MapPin } from "lucide-react";
import { DataTableColumnHeader } from "@/components/data-table";
import { Badge } from "@/components/ui/badge";
import type { Order } from "../../../common/schemas";

const dateFormatter = new Intl.DateTimeFormat("sv-SE", {
  dateStyle: "medium",
});

export const logisticsOrdersColumns: ColumnDef<Order>[] = [
  {
    accessorKey: "number",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Order" />
    ),
    cell: ({ row }) => (
      <Link
        to="/orders/$orderId"
        params={{ orderId: String(row.original.id) }}
        className="font-medium hover:underline"
      >
        {row.original.number}
      </Link>
    ),
    enableSorting: false,
  },
  {
    id: "destination",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Destination" />
    ),
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
    enableSorting: false,
  },
  {
    accessorKey: "status",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
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
    enableSorting: false,
  },
  {
    accessorKey: "estimatedDelivery",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Estimated Delivery" />
    ),
    cell: ({ row }) =>
      row.original.estimatedDelivery
        ? dateFormatter.format(new Date(row.original.estimatedDelivery))
        : "",
    enableSorting: false,
  },
];
