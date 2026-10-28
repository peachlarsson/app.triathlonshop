import { useQuery } from "@tanstack/react-query";
import { fetchDashboard } from "../data/api";

export const dashboardQueryKey = ["dashboard"] as const;

export function useDashboard() {
  return useQuery({
    queryKey: dashboardQueryKey,
    queryFn: ({ signal }) => fetchDashboard(signal),
  });
}
