import { useEffect, useState } from "react";
import { HubConnectionBuilder, type HubConnection } from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { negotiateOrderLiveUpdates } from "../data/api";

interface OrderUpdate {
  update?: boolean;
  Update?: boolean;
}

export function useOrderLiveUpdates(orderId: string) {
  const queryClient = useQueryClient();
  const [error, setError] = useState<string>();

  useEffect(() => {
    const abortController = new AbortController();
    let connection: HubConnection | undefined;

    const negotiate = () =>
      negotiateOrderLiveUpdates(abortController.signal);

    const connect = async () => {
      setError(undefined);
      try {
        const connectionInfo = await negotiate();
        if (abortController.signal.aborted) return;

        connection = new HubConnectionBuilder()
          .withUrl(connectionInfo.url, {
            accessTokenFactory: async () => (await negotiate()).accessToken,
          })
          .withAutomaticReconnect()
          .build();

        connection.on(`order${orderId}`, (update: OrderUpdate | null | undefined) => {
          if (
            !abortController.signal.aborted &&
            (update?.update ?? update?.Update) === true
          ) {
            void queryClient.invalidateQueries({
              queryKey: ["order", orderId],
            });
          }
        });
        connection.onreconnected(() => {
          if (!abortController.signal.aborted) {
            setError(undefined);
            void queryClient.invalidateQueries({
              queryKey: ["order", orderId],
            });
          }
        });
        connection.onreconnecting(() => {
          if (!abortController.signal.aborted) {
            setError("Order live updates are reconnecting.");
          }
        });
        connection.onclose(() => {
          if (!abortController.signal.aborted) {
            setError(
              "Order live updates disconnected. Reload the page to reconnect.",
            );
          }
        });

        await connection.start();
        if (!abortController.signal.aborted) {
          setError(undefined);
        }
      } catch (error) {
        if (!abortController.signal.aborted) {
          setError(
            error instanceof Error
              ? error.message
              : "Could not connect to order live updates.",
          );
        }
      }
    };

    void connect();

    return () => {
      abortController.abort();
      if (connection) {
        void connection.stop().catch((error: unknown) => {
          console.error("Could not stop order live updates.", error);
        });
      }
    };
  }, [orderId, queryClient]);

  return { error };
}
