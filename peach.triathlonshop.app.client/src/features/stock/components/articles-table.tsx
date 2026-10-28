import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { fetchArticlesPage } from "../data/api";
import { SortDirection, SortKey } from "../data/types";

const currency = new Intl.NumberFormat("sv-SE", {
  style: "currency",
  currency: "SEK",
});

export function ArticlesTable() {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const { data, error, isPending, isFetching, refetch } = useQuery({
    queryKey: ["inventory", "articles", page],
    queryFn: ({ signal }) =>
      fetchArticlesPage(
        {
          articleName: "",
          pageNumber: page,
          pageSize,
          sortKey: SortKey.ArticleName,
          sortDirection: SortDirection.Asc,
        },
        signal,
      ),
  });

  return (
    <section
      aria-label="articles"
      className="overflow-hidden rounded-lg border bg-card"
    >
      <Table aria-label="articles" aria-busy={isPending}>
        <TableHeader>
          <TableRow>
            <TableHead>Article name</TableHead>
            <TableHead>Article number</TableHead>
            <TableHead>Unit price</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {error ? (
            <TableRow>
              <TableCell
                colSpan={3}
                className="h-24 text-center text-muted-foreground"
              >
                <div className="space-y-2 py-4">
                  <p role="alert" className="text-sm text-destructive">
                    {error.message}
                  </p>
                  <Button
                    variant="outline"
                    onClick={() => void refetch()}
                    disabled={isFetching}
                  >
                    Retry
                  </Button>
                </div>
              </TableCell>
            </TableRow>
          ) : isPending ? (
            <TableRow>
              <TableCell colSpan={3} className="h-24 text-center">
                <span role="status">Loading articles...</span>
              </TableCell>
            </TableRow>
          ) : data && data.data.length > 0 ? (
            data.data.map((article) => (
              <TableRow key={article.id}>
                <TableCell className="font-medium">
                  {article.articleName}
                </TableCell>
                <TableCell>{article.articleNumber}</TableCell>
                <TableCell>{currency.format(article.unitPrice)}</TableCell>
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={3} className="h-24 text-center">
                No articles found.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>

      <div className="flex flex-wrap items-center justify-between gap-3 border-t p-4">
        <p className="text-sm text-muted-foreground" role="status">
          {error
            ? "Not available"
            : data
              ? `${data.totalCount.toLocaleString()} articles`
              : "Loading..."}
        </p>
        <div className="flex items-center gap-3">
          <Button
            variant="outline"
            size="sm"
            disabled={page === 1 || isFetching || !!error}
            onClick={() => setPage((value) => value - 1)}
          >
            Previous
          </Button>
          <span className="text-sm">
            Page {page} of {Math.max(1, data?.totalPages ?? 1)}
          </span>
          <Button
            variant="outline"
            size="sm"
            disabled={!data || page >= data.totalPages || isFetching || !!error}
            onClick={() => setPage((value) => value + 1)}
          >
            Next
          </Button>
        </div>
      </div>
    </section>
  );
}
