import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { CreateOrderRequestDto } from "../data/schema";
import {
  confirmOrder,
  createOrder,
  dispatchOrder,
  fetchOrder,
  fetchOrders,
  shipOrder,
} from "../data/api";
import { GetOrdersRequestDto } from "@/common/schemas/orders-request-dto";

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

export function useDispatchOrder(orderId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => dispatchOrder(orderId),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: ["order", String(orderId)] }),
        queryClient.invalidateQueries({ queryKey: ["orders"] }),
      ]),
  });
}

export function useShipOrder(orderId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => shipOrder(orderId),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: ["order", String(orderId)] }),
        queryClient.invalidateQueries({ queryKey: ["orders"] }),
      ]),
  });
}