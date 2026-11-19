import type { SortDirection } from "./sort";

export type OrdersRequestDto = {
  id?: number;
  orderNumber?: string;
  orderStatuses?: OrderStatus[];
  customerName?: string;
  articleName?: string;
  pageNumber: number;
  pageSize: number;
  sortKey: SortKey;
  sortDirection: SortDirection;
};

export enum OrderStatus {
  Created,
  Confirmed,
  Dispatched,
  Shipped,
  Cancelled,
}

export enum SortKey {
  Id,
  OrderNumber,
  CustomerName,
  OrderStatus,
  TotalPrice,
  TransportStatus,
  EstimatedDeliveryDate,
  PlacedAt,
  CreatedAt,
  UpdatedAt,
}
