import { useQuery } from "@tanstack/react-query";
import type { GetOrdersRequestDto } from "../data/schema";
import { fetchOrder, fetchOrders } from "../data/api";

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
    queryFn: ({ signal }) => fetchOrders(request, signal),
  });
}

export function useOrder(orderId: string) {
  return useQuery({
    queryKey: ["order", orderId],
    queryFn: ({ signal }) => fetchOrder(orderId, signal),
  });
}
