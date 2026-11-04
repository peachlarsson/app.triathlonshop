import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { CreateOrderRequestDto, GetOrdersRequestDto } from "../data/schema";
import { confirmOrder, createOrder, fetchOrder, fetchOrders } from "../data/api";

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

export function useCreateOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: CreateOrderRequestDto) => createOrder(params),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["orders"] }),
  });
}

export function useConfirmOrder(orderId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => confirmOrder(orderId),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: ["order", String(orderId)] }),
        queryClient.invalidateQueries({ queryKey: ["orders"] }),
      ]),
  });
}