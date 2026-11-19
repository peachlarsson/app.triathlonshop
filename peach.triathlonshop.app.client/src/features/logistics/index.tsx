import { getRouteApi } from "@tanstack/react-router";
import { useState } from "react";
import { type SortingState } from "@tanstack/react-table";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { LogisticsHeader } from "./components/logistics-header";
import { LogisticsOrdersTable } from "./components/logistics-orders-table";
import { useLogisticsOrders } from "./hooks/use-logistics-orders";
import { OrderStatus, SortKey as SortKeyOrders } from "@/common/schemas/orders-request-dto";
import { SortDirection } from "@/common/schemas/sort";
import { ColumnId } from "./data/schema";

const route = getRouteApi("/logistics");

const orderStatuses = [OrderStatus.Confirmed, OrderStatus.Dispatched, OrderStatus.Shipped];

const toSortKey = (columnId?: string): SortKeyOrders => {
  if (columnId === ColumnId.OrderNumber) return SortKeyOrders.OrderNumber;
  if (columnId === ColumnId.TransportStatus) return SortKeyOrders.TransportStatus;
  if (columnId === ColumnId.EstimatedDeliveryDate) return SortKeyOrders.EstimatedDeliveryDate;
  if (columnId === ColumnId.PlacedAt) return SortKeyOrders.PlacedAt;
  return SortKeyOrders.PlacedAt;
};

export function Logistics() {
  const search = route.useSearch();
  const navigate = route.useNavigate();
  const [sorting, setSorting] = useState<SortingState>([
    { id: ColumnId.PlacedAt, desc: true },
  ]);
  const sort = sorting[0];
  const {
    data: orders,
    error,
    isPending,
  } = useLogisticsOrders(
    {
      orderStatuses,
      pageNumber: search.page ?? 1,
      pageSize: search.pageSize ?? 10,
      sortKey: toSortKey(sort?.id),
      sortDirection: sort?.desc ? SortDirection.Desc : SortDirection.Asc,
    }
  );

  const totalCount = orders?.totalCount ?? 0;

  return (
    <>
      <Header>
        <div className="ms-auto">
          <ThemeSwitch />
        </div>
      </Header>
      <Main className="flex flex-1 flex-col gap-6">
        <LogisticsHeader
          totalOrders={totalCount}
          isLoading={isPending}
          hasError={Boolean(error)}
        />
        <LogisticsOrdersTable
          data={orders?.data ?? []}
          totalCount={totalCount}
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