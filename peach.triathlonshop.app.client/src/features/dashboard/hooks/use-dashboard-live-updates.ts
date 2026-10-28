import { useEffect, useState } from "react";
import { HubConnectionBuilder, type HubConnection } from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { dashboardQueryKey } from "./use-dashboard";
import { negotiateDashboardLiveUpdates } from "../data/api";

interface HomePageUpdate {
  update?: boolean;
  Update?: boolean;
}

export function useDashboardLiveUpdates() {
  const queryClient = useQueryClient();
  const [error, setError] = useState<string>();
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    const abortController = new AbortController();
    let connection: HubConnection | undefined;

    const refreshDashboard = () => {
      void queryClient.invalidateQueries({ queryKey: dashboardQueryKey });
    };

    const negotiate = () =>
      negotiateDashboardLiveUpdates(abortController.signal);

    const connect = async () => {
      try {
        const connectionInfo = await negotiate();
        if (abortController.signal.aborted) return;

        connection = new HubConnectionBuilder()
          .withUrl(connectionInfo.url, {
            accessTokenFactory: async () => (await negotiate()).accessToken,
          })
          .withAutomaticReconnect()
          .build();

        connection.on("homepage", (update: HomePageUpdate) => {
          if (
            !abortController.signal.aborted &&
            (update.update ?? update.Update)
          ) {
            refreshDashboard();
          }
        });
        connection.onreconnecting(() => {
          if (!abortController.signal.aborted) {
            setIsConnected(false);
            setError("Dashboard live updates are reconnecting.");
          }
        });
        connection.onreconnected(() => {
          if (!abortController.signal.aborted) {
            setIsConnected(true);
            setError(undefined);
            refreshDashboard();
          }
        });
        connection.onclose(() => {
          if (!abortController.signal.aborted) {
            setIsConnected(false);
            setError(
              "Dashboard live updates disconnected. Reload the page to reconnect.",
            );
          }
        });

        await connection.start();
        if (!abortController.signal.aborted) {
          setIsConnected(true);
          setError(undefined);
          refreshDashboard();
        }
      } catch (error) {
        if (!abortController.signal.aborted) {
          setError(
            error instanceof Error
              ? error.message
              : "Could not connect to dashboard live updates.",
          );
        }
      }
    };

    void connect();

    return () => {
      abortController.abort();
      if (connection) {
        void connection.stop().catch((error: unknown) => {
          console.error("Could not stop dashboard live updates.", error);
        });
      }
    };
  }, [queryClient]);

  return { error, isConnected };
}
