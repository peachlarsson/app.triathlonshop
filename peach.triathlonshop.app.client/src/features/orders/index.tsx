import { getRouteApi } from "@tanstack/react-router";
import { useState } from "react";
import { type SortingState } from "@tanstack/react-table";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import {
  type GetOrdersRequestDto,
  SortDirection,
  SortKey,
} from "./data/schema";
import { CreateOrderButton } from "./components/create-order-button";
import { OrdersTable } from "./components/orders-table";
import { useOrders } from "./hooks/use-orders";

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

  const { data: orders, error, isPending } = useOrders(request);

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
          </div>
          <CreateOrderButton />
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
