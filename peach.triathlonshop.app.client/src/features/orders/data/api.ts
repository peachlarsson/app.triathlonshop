import type { Order, PaginatedResponse } from "@/common/schemas";
import {
  type CreateOrderRequestDto,
  type GetArticlesRequestDto,
  type GetArticlesResponseDto,
} from "./schema";
import { SortDirection } from "@/common/schemas/sort";
import {
  type GetOrdersRequestDto,
  SortKey,
} from "@/common/schemas/orders-request-dto";

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
    sortKey: String(request.sortKey ?? SortKey.Id),
    sortDirection: String(request.sortDirection ?? SortDirection.Asc),
    includeOrderRows: String(request.includeOrderRows ?? false),
    includeCustomer: String(request.includeCustomer ?? false),
  });
  
  if (request.customerName) params.set("customerName", request.customerName);
  if (request.articleName) params.set("articleName", request.articleName);
  if (request.destinationLocation) params.set("destinationLocation", request.destinationLocation);

  const response = await fetch(`/api/orders?${params.toString()}`, { signal });
  if (!response.ok) {
    throw new Error(`Could not load orders (HTTP ${response.status}).`);
  }

  return (await response.json()) as PaginatedResponse<Order>;
}

export async function fetchOrder(
  orderId: number,
  signal: AbortSignal,
): Promise<Order | null> {
  if (!Number.isSafeInteger(orderId)) {
    throw new Error("Invalid order ID.");
  }

  const request: GetOrdersRequestDto = {
    id: orderId,
    includeOrderRows: true,
    includeCustomer: true,
    pageNumber: 1,
    pageSize: 100,
    sortKey: SortKey.Id,
    sortDirection: SortDirection.Asc,
  };
  const result = await fetchOrders(request, signal);
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

export async function dispatchOrder(orderId: number): Promise<void> {
  const response = await fetch(`/api/orders/${orderId}/dispatch`, {
    method: "POST",
  });

  if (!response.ok) {
    throw new Error(`Could not dispatch order (HTTP ${response.status}).`);
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
