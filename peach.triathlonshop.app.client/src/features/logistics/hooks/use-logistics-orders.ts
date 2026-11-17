import { useQuery } from "@tanstack/react-query";
import type { SortDirection } from "@/common/schemas/sort";
import type { SortKey } from "../data/schema";
import { fetchLogisticsOrders } from "../data/api";

export function useLogisticsOrders(
  page: number,
  pageSize: number,
  sortKey: SortKey,
  sortDirection: SortDirection,
) {
  return useQuery({
    queryKey: ["logistics-orders", page, pageSize, sortKey, sortDirection],
    queryFn: ({ signal }) =>
      fetchLogisticsOrders(page, pageSize, sortKey, sortDirection, signal),
  });
}
