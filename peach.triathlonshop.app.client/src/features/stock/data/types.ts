export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  totalPages: number;
}

export interface StockOverviewResponse {
  warehouses: StockOverviewWarehouse[];
  monthlyStockSales: { month: number; sales: number; purchased: number }[];
}

export interface StockOverviewWarehouse {
  id: number;
  name: string | null;
  totalArticles: number | null;
  inStock: number | null;
  outOfStock: number | null;
  awaitingDispatch: number | null;
  shipped: number | null;
}

export interface StockItem {
  id: number;
  articleNumber: string;
  articleName: string;
  quantity: number;
  warehouseName: string;
}

export interface Warehouse {
  id: number;
  warehouseName: string;
  city: string | null;
  countryName: string | null;
  inStockArticles: number;
  outOfStockArticles: number;
}

export interface ArticleDto {
  id: number;
  articleName: string;
  articleNumber: string;
  unitPrice: number;
}

export interface GetArticlesRequest {
  articleName?: string;
  pageNumber?: number;
  pageSize?: number;
  sortKey?: SortKey;
  sortDirection?: SortDirection;
}

export enum SortKey {
  Id = "Id",
  ArticleNumber = "ArticleNumber",
  ArticleName = "ArticleName",
  UnitPrice = "UnitPrice",
}

export enum SortDirection {
  Asc = "Asc",
  Desc = "Desc",
}

export interface WarehouseArticle {
  id: number;
  number: string;
  name: string;
  quantity: number;
}

export interface WarehouseDetailsResponse {
  warehouse: Warehouse;
  articles: PaginatedResponse<WarehouseArticle>;
  inStockArticles: number;
  outOfStockArticles: number;
  shippedOrders: number | null;
}
export interface WarehouseStockDto {
  warehouseId: number;
  warehouseName: string;
  location: { cityName: string; countryCode: string };
  articles: { articleId: number; quantity: number }[];
}

export interface ArticleWarehouseStock {
  warehouses: { id: number; name: string }[];
  quantities: { articleId: number; warehouseId: number; quantity: number }[];
}
