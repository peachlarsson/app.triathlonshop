import { useQuery } from "@tanstack/react-query";
import { fetchArticles } from "../data/api";
import type { GetArticlesRequestDto } from "../data/schema";

export function useArticlesInStock(
  params: GetArticlesRequestDto,
  enabled = true,
) {
  return useQuery({
    queryKey: ["order-articles-in-stock", params],
    enabled,
    queryFn: ({ signal }) => fetchArticles(params, signal),
  });
}