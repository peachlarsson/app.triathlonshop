import { useEffect } from "react";
import { useLocation, useNavigate } from "@tanstack/react-router";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Header } from "@/components/layout/header";
import { Main } from "@/components/layout/main";
import { ThemeSwitch } from "@/components/theme-switch";
import { StockOverview } from "./components/stock-overview";
import { StockChart } from "./components/stock-chart";
import { WarehouseCards } from "./components/warehouse-cards";
import { ArticlesTable } from "./components/articles-table";

export function Stock() {
  const hash = useLocation({ select: (location) => location.hash });
  const navigate = useNavigate();
  useEffect(() => {
    if (!hash) {
      void navigate({
        hash: "overview",
        replace: true,
        resetScroll: false,
        hashScrollIntoView: false,
      });
    }
  }, [hash, navigate]);
  const activeTab =
    hash === "warehouses" || hash === "articles" ? hash : "overview";

  return (
    <>
      <Header>
        <div className="ms-auto">
          <ThemeSwitch />
        </div>
      </Header>
      <Main className="flex flex-1 flex-col gap-4 sm:gap-6">
        <div className="mb-2 flex flex-wrap items-end justify-between gap-2">
          <div>
            <h2 className="text-2xl font-bold tracking-tight">Stock</h2>
          </div>
        </div>

        <Tabs
          orientation="vertical"
          value={activeTab}
          onValueChange={(value) => {
            void navigate({
              hash: value,
              resetScroll: false,
              hashScrollIntoView: false,
            });
          }}
          className="space-y-4"
        >
          <div className="w-full overflow-x-auto pb-2">
            <TabsList>
              <TabsTrigger value="overview">Overview</TabsTrigger>
              <TabsTrigger value="warehouses">Warehouses</TabsTrigger>
              <TabsTrigger value="articles">Articles</TabsTrigger>
            </TabsList>
          </div>

          <TabsContent value="overview" className="space-y-4">
            <StockOverview />
            <StockChart />
          </TabsContent>

          <TabsContent value="warehouses" className="space-y-4">
            <WarehouseCards />
          </TabsContent>

          <TabsContent value="articles" className="space-y-4">
            <ArticlesTable />
          </TabsContent>
        </Tabs>
      </Main>
    </>
  );
}
