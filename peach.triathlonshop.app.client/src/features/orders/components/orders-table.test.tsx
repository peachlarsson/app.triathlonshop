import { useState } from "react";
import { type SortingState } from "@tanstack/react-table";
import { describe, expect, it, vi } from "vitest";
import { render } from "vitest-browser-react";
import { type Order } from "../data/schema";
import { OrdersTable } from "./orders-table";

const orders: Order[] = [
  {
    id: 1,
    number: "ORD-001",
    status: 0,
    totalPrice: 100,
    customerId: 10,
    customerName: "Alice",
    createdAt: "2026-10-24T10:00:00Z",
  },
  {
    id: 2,
    number: "ORD-002",
    status: 1,
    totalPrice: 200,
    customerId: 20,
    customerName: "Bob",
    createdAt: "2026-10-24T11:00:00Z",
  },
];

describe("OrdersTable", () => {
  it("selects individual orders and all orders on the page", async () => {
    const screen = await render(
      <OrdersTable
        data={orders}
        totalCount={20}
        isLoading={false}
        search={{}}
        navigate={vi.fn()}
        sorting={[]}
        onSortingChange={vi.fn()}
      />,
    );

    const selectAll = screen.getByRole("checkbox", {
      name: "Select all",
      exact: true,
    });
    const selectRows = screen.getByRole("checkbox", {
      name: "Select row",
      exact: true,
    });
    await selectRows.nth(0).click();
    await expect.element(selectRows.nth(0)).toBeChecked();
    await expect.element(selectRows.nth(1)).not.toBeChecked();
    await selectAll.click();
    await expect.element(selectRows.nth(0)).toBeChecked();
    await expect.element(selectRows.nth(1)).toBeChecked();
    await selectAll.click();
    await expect.element(selectRows.nth(0)).not.toBeChecked();
    await expect.element(selectRows.nth(1)).not.toBeChecked();
  });

  it("uses the shared filter toolbar and server-side pagination", async () => {
    const navigate = vi.fn();
    const screen = await render(
      <OrdersTable
        data={orders}
        totalCount={20}
        isLoading={false}
        search={{}}
        navigate={navigate}
        sorting={[]}
        onSortingChange={vi.fn()}
      />,
    );

    await screen.getByPlaceholder("Filter orders by customer...").fill("Alice");
    const filterNavigation = navigate.mock.lastCall?.[0];
    expect(filterNavigation.search({ page: 2 })).toEqual({
      page: undefined,
      customerName: "Alice",
    });
    await expect
      .element(screen.getByText("Bob", { exact: true }))
      .toBeVisible();
    await screen.getByRole("button", { name: "Reset", exact: true }).click();
    expect(navigate.mock.lastCall?.[0].search({})).toEqual({
      page: undefined,
      customerName: undefined,
    });
    await screen
      .getByRole("button", { name: "Go to next page", exact: true })
      .click();
    expect(navigate.mock.lastCall?.[0].search({})).toEqual({
      page: 2,
      pageSize: undefined,
    });
  });

  it("uses the shared sortable column header", async () => {
    const onSortingChange = vi.fn();
    function SortedTable() {
      const [sorting, setSorting] = useState<SortingState>([]);
      return (
        <OrdersTable
          data={orders}
          totalCount={orders.length}
          isLoading={false}
          search={{}}
          navigate={vi.fn()}
          sorting={sorting}
          onSortingChange={(updater) => {
            const next =
              typeof updater === "function" ? updater(sorting) : updater;
            onSortingChange(next);
            setSorting(next);
          }}
        />
      );
    }

    const screen = await render(<SortedTable />);
    await screen.getByRole("button", { name: "Order", exact: true }).click();
    await screen.getByRole("menuitem", { name: "Desc", exact: true }).click();
    expect(onSortingChange).toHaveBeenCalledWith([
      { id: "number", desc: true },
    ]);
  });
});
