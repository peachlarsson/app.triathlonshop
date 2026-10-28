import { useQuery } from "@tanstack/react-query";
import {
  type Order,
  type PaginatedResponse,
} from "../../orders/data/schema";

export function useLogisticsOrders(page: number, pageSize: number) {
  return useQuery({
    queryKey: ["logistics-orders", page],
    queryFn: async ({ signal }): Promise<PaginatedResponse<Order>> => {
      const params = new URLSearchParams({
        pageNumber: String(page),
        pageSize: String(pageSize),
        sortKey: "createdAt",
        sortDirection: "desc",
      });
      const response = await fetch(`/api/orders?${params.toString()}`, {
        signal,
      });
      if (!response.ok) {
        throw new Error(
          `Could not load logistics orders (HTTP ${response.status}).`,
        );
      }

      return (await response.json()) as PaginatedResponse<Order>;
    },
  });
}
