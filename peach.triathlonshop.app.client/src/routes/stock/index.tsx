import { createFileRoute } from "@tanstack/react-router";
import { Stock } from "@/features/stock";

export const Route = createFileRoute("/stock/")({
  staticData: { appLayout: true },
  component: Stock,
});
