import { Button } from "@/components/ui/button";
import type { Order } from "../data/schema";
import { useDispatchOrder } from "../hooks/use-orders";

type DispatchOrderButtonProps = {
  order: Pick<Order, "id" | "status">;
};

export function DispatchOrderButton({ order }: DispatchOrderButtonProps) {
  const dispatchOrder = useDispatchOrder(order.id);

  if (order.status !== "Confirmed") return null;

  return (
    <div className="flex flex-col items-end gap-1">
      <Button
        type="button"
        size="sm"
        variant="secondary"
        disabled={dispatchOrder.isPending}
        onClick={() => dispatchOrder.mutate()}
      >
        {dispatchOrder.isPending ? "Dispatching..." : "Dispatch order"}
      </Button>
      {dispatchOrder.error && (
        <p className="text-sm text-destructive" role="alert">
          {dispatchOrder.error.message}
        </p>
      )}
    </div>
  );
}
