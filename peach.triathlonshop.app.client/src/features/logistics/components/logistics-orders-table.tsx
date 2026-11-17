import { useEffect, useState } from "react";
import {
  type SortingState,
  type Updater,
  type VisibilityState,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from "@tanstack/react-table";
import { cn } from "@/lib/utils";
import { type NavigateFn, useTableUrlState } from "@/hooks/use-table-url-state";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  DataTablePagination,
  DataTableViewOptions,
} from "@/components/data-table";
import type { Order } from "../../../common/schemas";
import { logisticsOrdersColumns as columns } from "./logistics-orders-columns";

type LogisticsOrdersTableProps = {
  data: Order[];
  totalCount: number;
  isLoading: boolean;
  error?: string;
  search: Record<string, unknown>;
  navigate: NavigateFn;
  sorting: SortingState;
  onSortingChange: (updater: Updater<SortingState>) => void;
};

export function LogisticsOrdersTable({
  data,
  totalCount,
  isLoading,
  error,
  search,
  navigate,
  sorting,
  onSortingChange,
}: LogisticsOrdersTableProps) {
  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({});
  const { pagination, onPaginationChange, ensurePageInRange } =
    useTableUrlState({
      search,
      navigate,
      pagination: { defaultPage: 1, defaultPageSize: 10 },
      globalFilter: { enabled: false },
      columnFilters: [],
    });

  // eslint-disable-next-line react-hooks/incompatible-library
  const table = useReactTable({
    data,
    columns,
    state: {
      sorting,
      pagination,
      columnVisibility,
    },
    getRowId: (order) => String(order.id),
    onColumnVisibilityChange: setColumnVisibility,
    onSortingChange,
    onPaginationChange,
    manualSorting: true,
    manualFiltering: true,
    manualPagination: true,
    rowCount: totalCount,
    getCoreRowModel: getCoreRowModel(),
  });

  useEffect(() => {
    ensurePageInRange(table.getPageCount());
  }, [table, ensurePageInRange]);

  return (
    <div className="flex flex-1 flex-col gap-4">
      <div className="flex items-center justify-end">
        <DataTableViewOptions table={table} />
      </div>
      <div className="overflow-hidden rounded-md border">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id} className="group/row">
                {headerGroup.headers.map((header) => (
                  <TableHead
                    key={header.id}
                    colSpan={header.colSpan}
                    className={cn(
                      "bg-background group-hover/row:bg-muted",
                      header.column.columnDef.meta?.className,
                      header.column.columnDef.meta?.thClassName,
                    )}
                  >
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext(),
                        )}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow key={row.id} className="group/row">
                  {row.getVisibleCells().map((cell) => (
                    <TableCell
                      key={cell.id}
                      className={cn(
                        "bg-background group-hover/row:bg-muted",
                        cell.column.columnDef.meta?.className,
                        cell.column.columnDef.meta?.tdClassName,
                      )}
                    >
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext(),
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={table.getVisibleLeafColumns().length}
                  className="h-24 text-center"
                  role={error ? "alert" : undefined}
                >
                  {error ??
                    (isLoading ? "Loading orders..." : "No orders found.")}
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
      <DataTablePagination table={table} className="mt-auto" />
    </div>
  );
}
