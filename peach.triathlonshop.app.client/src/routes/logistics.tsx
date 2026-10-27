import { createFileRoute } from "@tanstack/react-router";
import { Logistics } from "@/features/logistics";

export const Route = createFileRoute("/logistics")({
  staticData: { appLayout: true },
  validateSearch: (search: Record<string, unknown>) => ({
    page: typeof search.page === "number" && search.page > 0 ? search.page : 1,
  }),
  component: Logistics,
});
