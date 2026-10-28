import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { AnimatedNumber } from "./animated-number";
import { Overview } from "./overview";
import { RecentSales } from "./recent-sales";
import type { DashboardData } from "../data/schema";

interface DashboardOverviewProps {
  data: DashboardData | undefined;
  error: Error | null;
  isPending: boolean;
}

export function DashboardOverview({
  data,
  error,
  isPending,
}: DashboardOverviewProps) {
  const formatCurrency = (value: number) =>
    new Intl.NumberFormat("sv-SE", {
      style: "currency",
      currency: "SEK",
    }).format(value);

  return (
    <>
      {error && (
        <p role="alert" className="text-sm text-destructive">
          {error instanceof Error
            ? error.message
            : "Could not load dashboard data."}
        </p>
      )}
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              className="h-4 w-4 text-muted-foreground"
            >
              <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6" />
            </svg>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {isPending ? (
                "Loading..."
              ) : data ? (
                <AnimatedNumber
                  value={data.totalRevenue}
                  format={formatCurrency}
                />
              ) : (
                "—"
              )}
            </div>
            <p className="text-xs text-muted-foreground">
              Total revenue from shipped orders
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Customers</CardTitle>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              className="h-4 w-4 text-muted-foreground"
            >
              <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
              <circle cx="9" cy="7" r="4" />
              <path d="M22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75" />
            </svg>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {isPending ? (
                "Loading..."
              ) : data ? (
                <AnimatedNumber value={data.totalCustomers} />
              ) : (
                "—"
              )}
            </div>
            <p className="text-xs text-muted-foreground">
              Registered customers
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Orders</CardTitle>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              className="h-4 w-4 text-muted-foreground"
            >
              <rect width="20" height="14" x="2" y="5" rx="2" />
              <path d="M2 10h20" />
            </svg>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {isPending ? (
                "Loading..."
              ) : data ? (
                <AnimatedNumber value={data.totalOrders} />
              ) : (
                "—"
              )}
            </div>
            <p className="text-xs text-muted-foreground">Total orders</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Orders</CardTitle>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              className="h-4 w-4 text-muted-foreground"
            >
              <path d="M22 12h-4l-3 9L9 3l-3 9H2" />
            </svg>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {isPending ? (
                "Loading..."
              ) : data ? (
                <AnimatedNumber value={data.activeOrders} />
              ) : (
                "—"
              )}
            </div>
            <p className="text-xs text-muted-foreground">
              Created or confirmed
            </p>
          </CardContent>
        </Card>
      </div>
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-7">
        <Card className="col-span-1 lg:col-span-4">
          <CardHeader>
            <CardTitle>Overview</CardTitle>
            <CardDescription>Monthly revenue this year</CardDescription>
          </CardHeader>
          <CardContent className="ps-2">
            {isPending ? (
              <p className="py-8 text-center text-sm text-muted-foreground">
                Loading revenue data...
              </p>
            ) : data ? (
              <Overview monthlyRevenue={data.monthlyRevenue} />
            ) : (
              <p className="py-8 text-center text-sm text-muted-foreground">
                Revenue data is unavailable.
              </p>
            )}
          </CardContent>
        </Card>
        <Card className="col-span-1 lg:col-span-3">
          <CardHeader>
            <CardTitle>Recent Orders</CardTitle>
            <CardDescription>Orders placed in the last 30 days</CardDescription>
          </CardHeader>
          <CardContent>
            {isPending ? (
              <p className="py-8 text-center text-sm text-muted-foreground">
                Loading recent orders...
              </p>
            ) : data ? (
              <RecentSales orders={data.recentOrdersLast30Days} />
            ) : (
              <p className="py-8 text-center text-sm text-muted-foreground">
                Recent orders are unavailable.
              </p>
            )}
          </CardContent>
        </Card>
      </div>
    </>
  );
}
