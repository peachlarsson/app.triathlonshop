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
        OrderRows: 20,
        ArticlesCountIntervalStart: 1,
        ArticlesCountIntervalEnd: 10,
        QuantityIntervalStart: 1,
        QuantityIntervalEnd: 5,
        UseRandomOrderStatus: true,
        UseRandomCreatedDate: true,
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
