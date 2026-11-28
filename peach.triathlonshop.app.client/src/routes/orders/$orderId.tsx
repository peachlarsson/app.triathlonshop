import { createFileRoute } from "@tanstack/react-router";
import OrderDetails from "@/features/orders/details";

export const Route = createFileRoute("/orders/$orderId")({
  params: {
    parse: ({ orderId }) => {
      const id = Number(orderId);
      if (!/^-?\d+$/.test(orderId) || !Number.isSafeInteger(id)) {
        throw new Error("Invalid order ID.");
      }
      return { orderId: id };
    },
    stringify: ({ orderId }) => ({ orderId: String(orderId) }),
  },
  staticData: { appLayout: true },
  component: OrderDetails,
});
