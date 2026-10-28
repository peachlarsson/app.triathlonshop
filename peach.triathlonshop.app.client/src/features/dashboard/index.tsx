import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { Analytics } from "./components/analytics";
import { DashboardOverview } from "./components/dashboard-overview";
import { useCreateRandomOrders } from "./hooks/use-create-random-orders";
import { useDashboard } from "./hooks/use-dashboard";
import { useDashboardLiveUpdates } from "./hooks/use-dashboard-live-updates";

export function Dashboard() {
  const dashboardQuery = useDashboard();
  const { error: liveUpdateError, isConnected } = useDashboardLiveUpdates();
  const { error: createOrdersError } = useCreateRandomOrders(
    isConnected &&
      dashboardQuery.isSuccess &&
      dashboardQuery.isFetchedAfterMount,
  );
  const { data: dashboardData, error, isPending } = dashboardQuery;

  return (
    <>
      {/* ===== Top Heading ===== */}
      <Header>
        <div className="ms-auto">
          <ThemeSwitch />
        </div>
      </Header>

      {/* ===== Main ===== */}
      <Main>
        {createOrdersError && (
          <p role="alert" className="mb-4 text-sm text-destructive">
            {createOrdersError}
          </p>
        )}
        {liveUpdateError && (
          <p role="alert" className="mb-4 text-sm text-destructive">
            {liveUpdateError}
          </p>
        )}
        <Tabs
          orientation="vertical"
          defaultValue="overview"
          className="space-y-4"
        >
          <div className="w-full overflow-x-auto pb-2">
            <TabsList>
              <TabsTrigger value="overview">Overview</TabsTrigger>
            </TabsList>
          </div>
          <TabsContent value="overview" className="space-y-4">
            <DashboardOverview
              data={dashboardData}
              error={error}
              isPending={isPending}
            />
          </TabsContent>
          <TabsContent value="analytics" className="space-y-4">
            <Analytics />
          </TabsContent>
        </Tabs>
      </Main>
    </>
  );
}
