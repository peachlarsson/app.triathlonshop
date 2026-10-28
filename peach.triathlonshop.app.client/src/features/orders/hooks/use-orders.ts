import { useQuery } from "@tanstack/react-query";
import {
  type GetOrdersRequestDto,
  type Order,
  type PaginatedResponse,
  SortDirection,
  SortKey,
} from "../data/schema";

export function useOrders(request: GetOrdersRequestDto) {
  return useQuery({
    queryKey: [
      "orders",
      request.pageNumber,
      request.pageSize,
      request.customerName,
      request.articleName,
      request.sortKey,
      request.sortDirection,
    ],
    queryFn: async ({ signal }): Promise<PaginatedResponse<Order>> => {
      const params = new URLSearchParams({
        id: String(request.id ?? ""),
        pageNumber: String(request.pageNumber ?? 1),
        pageSize: String(request.pageSize ?? 10),
        customerName: request.customerName ?? "",
        articleName: request.articleName ?? "",
        sortKey: request.sortKey ?? SortKey.Id,
        sortDirection: request.sortDirection ?? SortDirection.Asc,
      });

      const response = await fetch(`/api/orders?${params.toString()}`, {
        signal,
      });
      if (!response.ok) {
        throw new Error(`Could not load orders (HTTP ${response.status}).`);
      }

      return (await response.json()) as PaginatedResponse<Order>;
    },
  });
}

export function useOrder(orderId: string) {
  return useQuery({
    queryKey: ["order", orderId],
    queryFn: async ({ signal }): Promise<Order | null> => {
      const params = new URLSearchParams({ id: orderId });
      const response = await fetch(`/api/orders?${params.toString()}`, {
        signal,
      });

      if (!response.ok) {
        throw new Error(`Could not load order (HTTP ${response.status}).`);
      }

      const result: PaginatedResponse<Order> = await response.json();
      return result.data[0] ?? null;
    },
  });
}
