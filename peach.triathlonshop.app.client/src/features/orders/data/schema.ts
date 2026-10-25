export type Order = {
  id: number;
  number: string;
  status: string;
  totalPrice: number;
  orderRowsCount?: number;
  customer: Customer;
  createdAt: string;
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
