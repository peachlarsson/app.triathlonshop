import { getRouteApi } from "@tanstack/react-router";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { LogisticsHeader } from "./components/logistics-header";
import { LogisticsOrdersTable } from "./components/logistics-orders-table";
import { useLogisticsOrders } from "./hooks/use-logistics-orders";

const route = getRouteApi("/logistics");

export function Logistics() {
  const search = route.useSearch();
  const navigate = route.useNavigate();
  const {
    data: orders,
    error,
    isPending,
  } = useLogisticsOrders(search.page ?? 1, search.pageSize ?? 10);

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
        />
      </Main>
    </>
  );
}