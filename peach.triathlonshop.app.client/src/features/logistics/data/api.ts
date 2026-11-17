import type { Order, PaginatedResponse } from "../../../common/schemas";
import type { SortDirection } from "@/common/schemas/sort";
import type { SortKey } from "./schema";

export async function fetchLogisticsOrders(
  page: number,
  pageSize: number,
  sortKey: SortKey,
  sortDirection: SortDirection,
  signal: AbortSignal,
): Promise<PaginatedResponse<Order>> {
  const params = new URLSearchParams({
    pageNumber: String(page),
    pageSize: String(pageSize),
    sortKey,
    sortDirection,
  });
  const response = await fetch(`/api/logistics?${params.toString()}`, { signal });
  if (!response.ok) {
    throw new Error(
      `Could not load logistics orders (HTTP ${response.status}).`,
    );
  }

  return (await response.json()) as PaginatedResponse<Order>;
}
