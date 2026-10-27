import { useQuery } from "@tanstack/react-query";
import {
  Bar,
  BarChart,
  LabelList,
  ResponsiveContainer,
  XAxis,
  YAxis,
} from "recharts";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { fetchStockChart } from "../data/api";

const monthNames = [
  "Jan",
  "Feb",
  "Mar",
  "Apr",
  "May",
  "Jun",
  "Jul",
  "Aug",
  "Sep",
  "Oct",
  "Nov",
  "Dec",
];

export function StockChart() {
  const year = new Date().getFullYear();
  const { data, error, isPending, isFetching, refetch } = useQuery({
    queryKey: ["inventory", "chart", year],
    queryFn: ({ signal }) => fetchStockChart(year, signal),
  });
  const chartData = [...(data?.months ?? [])]
    .sort((a, b) => a.month - b.month)
    .map(({ month, soldCount }) => ({
      name: monthNames[month - 1],
      count: soldCount,
    }));

  return (
    <Card>
      <CardHeader>
        <CardTitle>Articles sold - {year}</CardTitle>
      </CardHeader>
      <CardContent>
        <div
          className="flex h-80 min-w-0 flex-col items-center justify-center gap-3"
          aria-busy={isPending}
        >
          {error ? (
            <>
              <p role="alert" className="text-sm text-destructive">
                {error.message}
              </p>
              <Button
                variant="outline"
                disabled={isFetching}
                onClick={() => void refetch()}
              >
                Retry
              </Button>
            </>
          ) : isPending ? (
            <p role="status" className="text-sm text-muted-foreground">
              Loading...
            </p>
          ) : chartData.length ? (
            <>
              <p className="sr-only">
                {chartData
                  .map(
                    ({ name, count }) =>
                      `${name}: ${count.toLocaleString()} articles sold.`,
                  )
                  .join(" ")}
              </p>
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={chartData} margin={{ top: 24, right: 16 }}>
                  <XAxis
                    dataKey="name"
                    stroke="var(--muted-foreground)"
                    fontSize={12}
                    tickLine={false}
                    axisLine={false}
                    interval={0}
                  />
                  <YAxis
                    allowDecimals={false}
                    stroke="var(--muted-foreground)"
                    fontSize={12}
                    tickLine={false}
                    axisLine={false}
                  />
                  <Bar
                    dataKey="count"
                    fill="var(--primary)"
                    radius={[4, 4, 0, 0]}
                    maxBarSize={100}
                  >
                    <LabelList
                      dataKey="count"
                      position="top"
                      fill="var(--foreground)"
                    />
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </>
          ) : (
            <p className="text-sm text-muted-foreground">No data available.</p>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
