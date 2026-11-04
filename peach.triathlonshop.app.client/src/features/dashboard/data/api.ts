import type { DashboardData } from "./schema";

export interface SignalRConnectionInfo {
  url: string;
  accessToken: string;
}

export async function fetchDashboard(
  signal: AbortSignal,
): Promise<DashboardData> {
  const response = await fetch("/api/dashboard", { signal });
  if (response.status !== 200) {
    throw new Error(`Could not load dashboard data (HTTP ${response.status}).`);
  }

  return (await response.json()) as DashboardData;
}

export async function negotiateDashboardLiveUpdates(
  signal: AbortSignal,
): Promise<SignalRConnectionInfo> {
  const response = await fetch("/api/homepage/negotiation/HomePage", {
    signal,
  });
  if (response.status !== 200) {
    throw new Error(
      `Could not connect to dashboard live updates (HTTP ${response.status}).`,
    );
  }

  return (await response.json()) as SignalRConnectionInfo;
}

export async function createRandomOrders(): Promise<void> {
  const response = await fetch(
    "/api/dashboard/developer/create-random-orders",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        OrdersCountIntervalStart: 10,
        OrdersCountIntervalEnd: 10,
        ArticlesCountIntervalStart: 2,
        ArticlesCountIntervalEnd: 5,
        QuantityIntervalStart: 1,
        QuantityIntervalEnd: 20,
        UseRandomOrderStatus: true,
        UseRandomPlacedDate: true,
        UseRandomDelay: true,
        DelayMsIntervalStart: 0,
        DelayMsIntervalEnd: 3000,
      }),
    },
  );
  if (!response.ok) {
    throw new Error(
      `Could not create random orders (HTTP ${response.status}).`,
    );
  }
}
