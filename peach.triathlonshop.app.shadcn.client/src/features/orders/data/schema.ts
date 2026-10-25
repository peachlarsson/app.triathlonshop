export type Order = {
  id: number;
  number: string;
  status: number | string;
  totalPrice: number;
  orderRowsCount?: number;
  customerId: number;
  customerName: string;
  createdAt: string;
};

export type PaginatedResponse<T> = {
  data: T[];
  totalCount: number;
};

export function getOrderStatus(status: number | string) {
  const statuses = ["Created", "Confirmed", "Shipped", "Cancelled"];
  return typeof status === "number" ? (statuses[status] ?? "Unknown") : status;
}
