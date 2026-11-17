import { useQuery } from "@tanstack/react-query";
import { fetchLogisticsOrders } from "../data/api";

export function useLogisticsOrders(page: number, pageSize: number) {
  return useQuery({
    queryKey: ["logistics-orders", page, pageSize],
    queryFn: ({ signal }) => fetchLogisticsOrders(page, pageSize, signal),
  });
}
