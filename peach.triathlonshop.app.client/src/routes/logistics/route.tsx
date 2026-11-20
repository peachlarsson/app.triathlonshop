import z from "zod";
import { createFileRoute } from "@tanstack/react-router";
import { Logistics } from "@/features/logistics";

const logisticsSearchSchema = z.object({
  page: z.number().optional().catch(1),
  pageSize: z.number().optional().catch(10),
  destinationLocation: z.string().optional().catch(""),
});

export const Route = createFileRoute("/logistics")({
  staticData: { appLayout: true },
  validateSearch: logisticsSearchSchema,
  component: Logistics,
});
