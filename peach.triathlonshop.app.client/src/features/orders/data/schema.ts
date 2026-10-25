export type Order = {
  id: number;
  number: string;
  status: string;
  totalPrice: number;
  orderRows: OrderRow[];
  orderRowsCount?: number;
  customer: Customer;
  createdAt: string;
};

export type OrderRow = {
  id: number;
  lineNumber: number;
  articleName: string;
  unitPrice: number;
  quantity: number;
  rowPrice: number;
  createdAt: string;
  updatedAt?: string;
};

export type Customer = {
  id: number;
  email?: string;
  name?: string;
  createdAt: string;
  updatedAt?: string;
};

export type PaginatedResponse<T> = {
  data: T[];
  totalCount: number;
};

export type GetOrdersRequestDto = {
    id?: number;
    orderNumber?: string;
    customerName?: string;
    articleName?: string;
    pageNumber?: number;
    pageSize?: number;
    sortKey?: SortKey;
    sortDirection?: SortDirection;
};

export enum SortKey {
    Id = "id",
    OrderNumber = "orderNumber",
    CustomerName = "customerName",
    CreatedAt = "createdAt",
    UpdatedAt = "updatedAt",
}

export enum SortDirection {
    Asc = "asc",
    Desc = "desc",
}