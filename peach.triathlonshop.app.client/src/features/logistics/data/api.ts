import type { Order, PaginatedResponse } from "../../../common/schemas";

export async function fetchLogisticsOrders(
  page: number,
  pageSize: number,
  signal: AbortSignal,
): Promise<PaginatedResponse<Order>> {
  const params = new URLSearchParams({
    pageNumber: String(page),
    pageSize: String(pageSize),
    sortKey: "createdAt",
    sortDirection: "desc",
  });
  const response = await fetch(`/api/orders?${params.toString()}`, { signal });
  if (!response.ok) {
    throw new Error(
      `Could not load logistics orders (HTTP ${response.status}).`,
    );
  }

  return (await response.json()) as PaginatedResponse<Order>;
}
