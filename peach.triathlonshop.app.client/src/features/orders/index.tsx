import { useQuery } from "@tanstack/react-query";
import { getRouteApi } from "@tanstack/react-router";
import { useState } from "react";
import { type SortingState } from "@tanstack/react-table";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { type Order, type PaginatedResponse } from "./data/schema";
import { OrdersTable } from "./components/orders-table";

const route = getRouteApi("/orders/");

export function Orders() {
  const search = route.useSearch();
  const navigate = route.useNavigate();
  const [sorting, setSorting] = useState<SortingState>([]);
  const page = search.page ?? 1;
  const pageSize = search.pageSize ?? 10;
  const customerName = search.customerName ?? "";
  const sort = sorting[0];
  const sortBy =
    sort?.id === "number"
      ? "Number"
      : sort?.id === "totalPrice"
        ? "TotalPrice"
        : sort?.id === "customerName"
          ? "CustomerName"
          : sort?.id === "createdAt"
            ? "CreatedAt"
            : "Id";
  const sortOrder = sort?.desc ? "Desc" : "Asc";
  const {
    data: orders,
    error,
    isPending,
  } = useQuery({
    queryKey: ["orders", page, pageSize, customerName, sortBy, sortOrder],
    queryFn: async (): Promise<PaginatedResponse<Order>> => {
      const params = new URLSearchParams({
        pageNumber: String(page),
        pageSize: String(pageSize),
        customerName,
        sortBy,
        sortOrder,
      });
      const response = await fetch(`/api/orders?${params.toString()}`);
      if (!response.ok) {
        throw new Error(`Could not load orders (HTTP ${response.status}).`);
      }
      return (await response.json()) as PaginatedResponse<Order>;
    },
  });

  return (
    <>
      <Header>
        <div className="ms-auto">
          <ThemeSwitch />
        </div>
      </Header>
      <Main className="flex flex-1 flex-col gap-4 sm:gap-6">
        <div className="flex flex-wrap items-end justify-between gap-2">
          <div>
            <h2 className="text-2xl font-bold tracking-tight">Orders</h2>
            <p className="text-muted-foreground">Browse and manage orders.</p>
          </div>
        </div>
        <OrdersTable
          data={orders?.data ?? []}
          totalCount={orders?.totalCount ?? 0}
          isLoading={isPending}
          error={error instanceof Error ? error.message : undefined}
          search={search}
          navigate={navigate}
          sorting={sorting}
          onSortingChange={setSorting}
        />
      </Main>
    </>
  );
}
