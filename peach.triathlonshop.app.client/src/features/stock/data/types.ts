export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  totalPages: number;
}

export interface StockOverview {
  totalWarehouses: number;
  inStockArticles: number;
  outOfStockArticles: number;
  totalUnits: number;
  locations: { city: string; countryName: string }[] | null;
  outgoingOrders:
    | {
        warehouseId: number;
        warehouseName: string;
        awaitingDispatch: number;
        shipped: number;
      }[]
    | null;
}

export interface StockItem {
  id: number;
  articleNumber: string;
  articleName: string;
  quantity: number;
  warehouseName: string;
}

export interface StockChartResponse {
  months: { month: number; soldCount: number }[];
}

export interface Warehouse {
  id: number;
  warehouseName: string;
  city: string | null;
  countryName: string | null;
  inStockArticles: number;
  outOfStockArticles: number;
}

export interface Article {
  id: number;
  number: string;
  name: string;
  unitPrice: number;
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
  articles: { articleId: number; quantity: number }[];
}
