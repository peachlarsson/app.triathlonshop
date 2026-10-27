import { createFileRoute } from "@tanstack/react-router";
import { WarehouseDetails } from "@/features/stock/warehouse-details";

export const Route = createFileRoute("/stock/warehouse/$warehouseId")({
  staticData: { appLayout: true },
  component: WarehouseDetails,
});
