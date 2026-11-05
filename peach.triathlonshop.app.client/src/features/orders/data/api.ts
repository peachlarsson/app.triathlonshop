import {
  type CreateOrderRequestDto,
  type GetArticlesRequestDto,
  type GetArticlesResponseDto,
  type GetOrdersRequestDto,
  type Order,
  type PaginatedResponse,
  SortDirection,
  SortKey,
} from "./schema";

export interface OrderLiveUpdateConnectionInfo {
  url: string;
  accessToken: string;
}

export async function negotiateOrderLiveUpdates(
  signal: AbortSignal,
): Promise<OrderLiveUpdateConnectionInfo> {
  const response = await fetch("/api/orders/negotiation/orders", { signal });
  if (!response.ok) {
    throw new Error(
      `Could not connect to order live updates (HTTP ${response.status}).`,
    );
  }

  return (await response.json()) as OrderLiveUpdateConnectionInfo;
}

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
  const order = result.data[0];
  return order
    ? {
        ...order,
        invoices: order.invoices ?? [],
        transportOrders: order.transportOrders ?? [],
        notifications: order.notifications ?? [],
      }
    : null;
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

export async function createOrder(
  params: CreateOrderRequestDto,
  signal?: AbortSignal,
): Promise<void> {
  const response = await fetch("/api/orders", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(params),
    signal,
  });

  if (!response.ok) {
    throw new Error(`Could not create order (HTTP ${response.status}).`);
  }
}

export async function confirmOrder(orderId: number): Promise<void> {
  const response = await fetch(`/api/orders/${orderId}/confirm`, {
    method: "POST",
  });

  if (!response.ok) {
    throw new Error(`Could not confirm order (HTTP ${response.status}).`);
  }
}

export async function shipOrder(orderId: number): Promise<void> {
  const response = await fetch(`/api/orders/${orderId}/ship`, {
    method: "POST",
  });

  if (!response.ok) {
    throw new Error(`Could not mark order as shipped (HTTP ${response.status}).`);
  }
}
