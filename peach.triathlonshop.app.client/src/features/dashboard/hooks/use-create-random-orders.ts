import { useEffect, useRef, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { createRandomOrders } from "../data/api";
import { dashboardQueryKey } from "./use-dashboard";

export function useCreateRandomOrders(enabled: boolean) {
  const queryClient = useQueryClient();
  const [error, setError] = useState<string>();
  const request = useRef<Promise<void> | null>(null);

  useEffect(() => {
    if (!enabled) return;

    let active = true;

    // Keep the request across Strict Mode's effect replay, but not a new visit.
    request.current ??= createRandomOrders().then(() => {
      void queryClient.invalidateQueries({ queryKey: dashboardQueryKey });
    });
    void request.current.catch((error: unknown) => {
      if (active) {
        setError(
          error instanceof Error
            ? error.message
            : "Could not create random orders.",
        );
      }
    });

    return () => {
      active = false;
    };
  }, [enabled, queryClient]);

  return { error };
}
