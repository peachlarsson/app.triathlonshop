import { getRouteApi } from "@tanstack/react-router";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { LogisticsHeader } from "./components/logistics-header";
import { OrderDestinationsTable } from "./components/order-destinations-table";
import { useLogisticsOrders } from "./hooks/use-logistics-orders";

const route = getRouteApi("/logistics");
const pageSize = 10;

export function Logistics() {
  const { page } = route.useSearch();
  const navigate = route.useNavigate();
  const { data: orders, error, isPending } = useLogisticsOrders(page, pageSize);

  const totalCount = orders?.totalCount ?? 0;
  const pageCount = Math.ceil(totalCount / pageSize);

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
        <OrderDestinationsTable
          orders={orders?.data ?? []}
          page={page}
          pageCount={pageCount}
          pageSize={pageSize}
          totalCount={totalCount}
          isLoading={isPending}
          error={error instanceof Error ? error.message : undefined}
          onPageChange={(nextPage) =>
            navigate({ search: (previous) => ({ ...previous, page: nextPage }) })
          }
        />
      </Main>
    </>
  );
}