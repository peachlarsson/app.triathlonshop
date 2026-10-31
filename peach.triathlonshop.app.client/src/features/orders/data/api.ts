import {
  type GetArticlesRequestDto,
  type GetArticlesResponseDto,
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

export async function fetchArticles(
  params: GetArticlesRequestDto,
  signal: AbortSignal,
): Promise<GetArticlesResponseDto> {
  const searchParams = new URLSearchParams({
    articleName: params.articleName ?? "",
    pageSize: params.pageSize?.toString() ?? "10",
  });
  const response = await fetch(`/api/stock/articles?${searchParams.toString()}`, {
    signal,
  });

  if (!response.ok) {
    throw new Error(`Could not load articles (HTTP ${response.status}).`);
  }

  return (await response.json()) as GetArticlesResponseDto;
}