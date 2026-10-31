export type Order = {
  id: number;
  number: string;
  status: string;
  estimatedDelivery?: string;
  totalPrice: number;
  orderRows: OrderRow[];
  orderRowsCount?: number;
  customer: Customer;
  destinationLocation: Location;
  createdAt: string;
};

export type Location = {
  countryCode: string;
  countryName: string;
  city: string;
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
  totalPrice?: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
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
    OrderStatus = "orderStatus",
    TotalPrice = "totalPrice",
    CreatedAt = "createdAt",
    UpdatedAt = "updatedAt",
}

export enum SortDirection {
    Asc = "asc",
    Desc = "desc",
}

// Fetch articles

export type GetArticlesRequestDto = {
    articleName?: string;
    pageSize?: number;
}

export type ArticleDto = {
    id: number;
    articleName: string;
    unitPrice: number;
}

export type GetArticlesResponseDto = PaginatedResponse<ArticleDto>;