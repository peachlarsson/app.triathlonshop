import { Button } from "@/components/ui/button";
import type { Order } from "../data/schema";
import { useShipOrder } from "../hooks/use-orders";

type ShipOrderButtonProps = {
  order: Pick<Order, "id" | "status">;
};

export function ShipOrderButton({ order }: ShipOrderButtonProps) {
  const shipOrder = useShipOrder(order.id);

  if (order.status !== "Confirmed") return null;

  return (
    <div className="flex flex-col items-end gap-1">
      <Button
        type="button"
        size="sm"
        className="bg-green-700 text-white hover:bg-green-800"
        disabled={shipOrder.isPending}
        onClick={() => shipOrder.mutate()}
      >
        {shipOrder.isPending ? "Shipping..." : "Ship order"}
      </Button>
      {shipOrder.error && (
        <p className="text-sm text-destructive" role="alert">
          {shipOrder.error.message}
        </p>
      )}
    </div>
  );
}
