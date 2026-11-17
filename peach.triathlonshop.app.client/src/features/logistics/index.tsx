import { getRouteApi } from "@tanstack/react-router";
import { useState } from "react";
import { type SortingState } from "@tanstack/react-table";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { LogisticsHeader } from "./components/logistics-header";
import { LogisticsOrdersTable } from "./components/logistics-orders-table";
import { useLogisticsOrders } from "./hooks/use-logistics-orders";
import { SortKey } from "./data/schema";
import { SortDirection } from "@/common/schemas/sort";

const route = getRouteApi("/logistics");

const toSortKey = (columnId?: string): SortKey => {
  if (columnId === "orderNumber") return SortKey.OrderNumber;
  if (columnId === "status") return SortKey.TransportOrderStatus;
  if (columnId === "estimatedDelivery") return SortKey.EstimatedDeliveryDate;
  if (columnId === "placedAt") return SortKey.PlacedAt;
  return SortKey.CreatedAt;
};

export function Logistics() {
  const search = route.useSearch();
  const navigate = route.useNavigate();
  const [sorting, setSorting] = useState<SortingState>([
    { id: "placedAt", desc: true },
  ]);
  const sort = sorting[0];
  const {
    data: orders,
    error,
    isPending,
  } = useLogisticsOrders(
    search.page ?? 1,
    search.pageSize ?? 10,
    toSortKey(sort?.id),
    sort?.desc ? SortDirection.Desc : SortDirection.Asc,
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