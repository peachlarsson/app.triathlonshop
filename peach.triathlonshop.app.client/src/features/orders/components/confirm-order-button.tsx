import { Button } from "@/components/ui/button";
import type { Order } from "../data/schema";
import { useConfirmOrder } from "../hooks/use-orders";

type ConfirmOrderButtonProps = {
  order: Pick<Order, "id" | "status">;
};

export function ConfirmOrderButton({ order }: ConfirmOrderButtonProps) {
  const confirmOrder = useConfirmOrder(order.id);

  if (order.status !== "Created") return null;

  return (
    <div className="flex flex-col items-end gap-1">
      <Button
        type="button"
        size="sm"
        className="bg-green-700 text-white hover:bg-green-800"
        disabled={confirmOrder.isPending}
        onClick={() => confirmOrder.mutate()}
      >
        {confirmOrder.isPending ? "Confirming..." : "Confirm order"}
      </Button>
      {confirmOrder.error && (
        <p className="text-sm text-destructive" role="alert">
          {confirmOrder.error.message}
        </p>
      )}
    </div>
  );
}
