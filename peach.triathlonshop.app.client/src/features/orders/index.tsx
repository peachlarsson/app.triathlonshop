import { useQuery } from "@tanstack/react-query";
import { getRouteApi } from "@tanstack/react-router";
import { useState } from "react";
import { type SortingState } from "@tanstack/react-table";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import {
  type GetOrdersRequestDto,
  type Order,
  type PaginatedResponse,
  SortDirection,
  SortKey,
} from "./data/schema";
import { OrdersTable } from "./components/orders-table";

const route = getRouteApi("/orders/");

const toSortKey = (columnId?: string): SortKey => {
  if (columnId === "id") return SortKey.Id;
  if (columnId === "number") return SortKey.OrderNumber;
  if (columnId === "customerName") return SortKey.CustomerName;
  if (columnId === "status") return SortKey.OrderStatus;
  if (columnId === "totalPrice") return SortKey.TotalPrice;
  return SortKey.CreatedAt;
};

export function Orders() {
  const search = route.useSearch();
  const navigate = route.useNavigate();
  const [sorting, setSorting] = useState<SortingState>([
    { id: "createdAt", desc: true },
  ]);

  const page = search.page ?? 1;
  const pageSize = search.pageSize ?? 10;
  const customerName = search.customerName ?? "";
  const articleName = search.articleName ?? "";
  const sort = sorting[0];
  const request: GetOrdersRequestDto = {
    customerName,
    articleName,
    pageNumber: page,
    pageSize,
    sortKey: toSortKey(sort?.id),
    sortDirection: sort?.desc ? SortDirection.Desc : SortDirection.Asc,
  };

  const {
    data: orders,
    error,
    isPending,
  } = useQuery({
    queryKey: [
      "orders",
      request.pageNumber,
      request.pageSize,
      request.customerName,
      request.articleName,
      request.sortKey,
      request.sortDirection,
    ],
    queryFn: async (): Promise<PaginatedResponse<Order>> => {
      const params = new URLSearchParams({
        id: String(request.id ?? ""),
        pageNumber: String(request.pageNumber ?? 1),
        pageSize: String(request.pageSize ?? 10),
        customerName: request.customerName ?? "",
        articleName: request.articleName ?? "",
        sortKey: request.sortKey ?? SortKey.Id,
        sortDirection: request.sortDirection ?? SortDirection.Asc,
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
