import { createFileRoute } from "@tanstack/react-router";
import OrderDetails from "@/features/orders/details";

export const Route = createFileRoute("/orders/$orderId")({
  staticData: { appLayout: true },
  component: OrderDetails,
});
