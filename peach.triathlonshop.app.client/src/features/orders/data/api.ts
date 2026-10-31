import {
  type GetOrdersRequestDto,
  type Order,
  type PaginatedResponse,
  SortDirection,
  SortKey,
} from "./schema";

export async function fetchOrders(
  request: GetOrdersRequestDto,
  signal: AbortSignal,
): Promise<PaginatedResponse<Order>> {
  const params = new URLSearchParams({
    id: String(request.id ?? ""),
    pageNumber: String(request.pageNumber ?? 1),
    pageSize: String(request.pageSize ?? 10),
    customerName: request.customerName ?? "",
    articleName: request.articleName ?? "",
    sortKey: request.sortKey ?? SortKey.Id,
    sortDirection: request.sortDirection ?? SortDirection.Asc,
  });

  const response = await fetch(`/api/orders?${params.toString()}`, { signal });
  if (!response.ok) {
    throw new Error(`Could not load orders (HTTP ${response.status}).`);
  }

  return (await response.json()) as PaginatedResponse<Order>;
}

export async function fetchOrder(
  orderId: string,
  signal: AbortSignal,
): Promise<Order | null> {
  const params = new URLSearchParams({ id: orderId });
  const response = await fetch(`/api/orders?${params.toString()}`, { signal });

  if (!response.ok) {
    throw new Error(`Could not load order (HTTP ${response.status}).`);
  }

  const result = (await response.json()) as PaginatedResponse<Order>;
  return result.data[0] ?? null;
}
