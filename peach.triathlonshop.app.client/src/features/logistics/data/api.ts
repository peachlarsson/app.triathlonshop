import type { Order, PaginatedResponse } from "../../../common/schemas";
import { OrderStatus, SortKey, type OrdersRequestDto } from "@/common/schemas/orders-request-dto";

export async function fetchLogisticsOrders(
  request: OrdersRequestDto,
  signal: AbortSignal,
): Promise<PaginatedResponse<Order>> {
  const params = new URLSearchParams({
    pageNumber: String(request.pageNumber),
    pageSize: String(request.pageSize),
    sortKey: SortKey[request.sortKey],
    sortDirection: request.sortDirection,
  });

  if (request.id !== undefined) params.set("id", String(request.id));
  if (request.orderNumber !== undefined) params.set("orderNumber", request.orderNumber);
  if (request.customerName !== undefined) params.set("customerName", request.customerName);
  if (request.articleName !== undefined) params.set("articleName", request.articleName);

  request.orderStatuses?.forEach((status) =>
    params.append("orderStatuses", OrderStatus[status]),
  );
  
  const response = await fetch(`/api/logistics?${params.toString()}`, { signal });
  if (!response.ok) {
    throw new Error(
      `Could not load logistics orders (HTTP ${response.status}).`,
    );
  }

  return (await response.json()) as PaginatedResponse<Order>;
}
