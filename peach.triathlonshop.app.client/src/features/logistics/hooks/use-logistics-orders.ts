import { useQuery } from "@tanstack/react-query";
import { fetchLogisticsOrders } from "../data/api";
import type { OrdersRequestDto } from "@/common/schemas/orders-request-dto";

export function useLogisticsOrders(
  request: OrdersRequestDto
) {
  return useQuery({
    queryKey: ["logistics-orders", request],
    queryFn: ({ signal }) =>
      fetchLogisticsOrders(request, signal),
  });
}
