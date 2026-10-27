import { useQuery } from "@tanstack/react-query";
import { getRouteApi } from "@tanstack/react-router";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { type Order, type PaginatedResponse } from "../orders/data/schema";
import { LogisticsHeader } from "./components/logistics-header";
import { OrderDestinationsTable } from "./components/order-destinations-table";

const route = getRouteApi("/logistics");
const pageSize = 10;

export function Logistics() {
  const { page } = route.useSearch();
  const navigate = route.useNavigate();
  const {
    data: orders,
    error,
    isPending,
  } = useQuery({
    queryKey: ["logistics-orders", page],
    queryFn: async (): Promise<PaginatedResponse<Order>> => {
      const params = new URLSearchParams({
        pageNumber: String(page),
        pageSize: String(pageSize),
        sortKey: "createdAt",
        sortDirection: "desc",
      });
      const response = await fetch(`/api/orders?${params.toString()}`);
      if (!response.ok) {
        throw new Error(`Could not load logistics orders (HTTP ${response.status}).`);
      }

      return (await response.json()) as PaginatedResponse<Order>;
    },
  });

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