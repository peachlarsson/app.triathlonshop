export type PaginatedResponse<T> = {
  data: T[];
  totalCount: number;
  totalPrice?: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
};