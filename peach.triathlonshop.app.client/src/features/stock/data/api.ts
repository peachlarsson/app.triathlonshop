import type {
  ArticleDto,
  PaginatedResponse,
  StockItem,
  StockChartResponse,
  StockOverview,
  Warehouse,
  WarehouseArticle,
  WarehouseDetailsResponse,
  WarehouseStockDto,
  GetArticlesRequest,
} from "./types";

const pageSize = 20;

async function loadInventory(signal: AbortSignal) {
  const [stock, catalog] = await Promise.all([
    fetchInventory<WarehouseStockDto[]>("/api/stock/all-warehouses", signal),
    (async () => {
      const articles: ArticleDto[] = [];
      let page = 1;
      let totalPages: number;
      do {
        const result = await fetchInventory<PaginatedResponse<ArticleDto>>(
          `/api/articles?pageNumber=${page}&pageSize=100`,
          signal,
        );
        articles.push(...result.data);
        totalPages = result.totalPages;
        page++;
      } while (page <= totalPages);
      return articles;
    })(),
  ]);
  const articleById = new Map(catalog.map((article) => [article.id, article]));
  const assignedByWarehouse = new Map<number, Map<number, number>>();
  const zeroStockByWarehouse = new Map<number, number>();
  for (const warehouse of stock) {
    let quantities = assignedByWarehouse.get(warehouse.warehouseId);
    if (!quantities) {
      quantities = new Map();
      assignedByWarehouse.set(warehouse.warehouseId, quantities);
    }
    for (const article of warehouse.articles) {
      if (article.quantity === 0) {
        zeroStockByWarehouse.set(
          warehouse.warehouseId,
          (zeroStockByWarehouse.get(warehouse.warehouseId) ?? 0) + 1,
        );
      }
      quantities.set(
        article.articleId,
        (quantities.get(article.articleId) ?? 0) + article.quantity,
      );
    }
  }
  const warehouses: Warehouse[] = [];
  const articlesByWarehouse = new Map<number, WarehouseArticle[]>();
  const stockItems: StockItem[] = [];
  const globalQuantities = new Map(catalog.map((article) => [article.id, 0]));
  for (const [warehouseId, quantities] of [...assignedByWarehouse].sort(
    ([a], [b]) => a - b,
  )) {
    const articles = [...quantities]
      .map(([id, quantity]) => {
        const article = articleById.get(id);
        return {
          id,
          number: article?.articleNumber ?? String(id),
          name: article?.articleName ?? `Article ${id} (details not available)`,
          quantity,
        };
      })
      .sort((a, b) => a.name.localeCompare(b.name) || a.id - b.id);
    const inStockArticles = articles.filter(
      (article) => article.quantity > 0,
    ).length;
    const warehouse: Warehouse = {
      id: warehouseId,
      warehouseName: `Warehouse ${warehouseId}`,
      city: null,
      countryName: null,
      inStockArticles,
      outOfStockArticles: zeroStockByWarehouse.get(warehouseId) ?? 0,
    };
    warehouses.push(warehouse);
    articlesByWarehouse.set(warehouseId, articles);
    for (const article of articles) {
      stockItems.push({
        id: stockItems.length,
        articleNumber: article.number,
        articleName: article.name,
        quantity: article.quantity,
        warehouseName: warehouse.warehouseName,
      });
      globalQuantities.set(
        article.id,
        (globalQuantities.get(article.id) ?? 0) + article.quantity,
      );
    }
  }
  const inStockArticles = [...globalQuantities.values()].filter(
    (quantity) => quantity > 0,
  ).length;
  const overview: StockOverview = {
    totalWarehouses: warehouses.length,
    inStockArticles,
    outOfStockArticles: [...zeroStockByWarehouse.values()].reduce(
      (sum, count) => sum + count,
      0,
    ),
    totalUnits: stockItems.reduce((sum, item) => sum + item.quantity, 0),
    locations: null,
    outgoingOrders: null,
  };
  return { warehouses, articlesByWarehouse, stockItems, overview };
}

export async function fetchWarehouse(
  warehouseId: string,
  page: number,
  signal: AbortSignal,
): Promise<WarehouseDetailsResponse> {
  const inventory = await loadInventory(signal);
  const warehouse = inventory.warehouses.find(
    (item) => String(item.id) === warehouseId,
  );
  const articles = warehouse && inventory.articlesByWarehouse.get(warehouse.id);
  if (!warehouse || !articles) {
    throw new WarehouseNotFoundError(
      "Warehouse not found in the stock response.",
    );
  }
  return {
    warehouse,
    articles: paginate(articles, page),
    inStockArticles: warehouse.inStockArticles,
    outOfStockArticles: warehouse.outOfStockArticles,
    shippedOrders: null,
  };
}

export async function fetchOverview(
  signal: AbortSignal,
): Promise<StockOverview> {
  return (await loadInventory(signal)).overview;
}

export async function fetchStockPage(
  page: number,
  signal: AbortSignal,
): Promise<PaginatedResponse<StockItem>> {
  return paginate((await loadInventory(signal)).stockItems, page);
}

export function fetchStockChart(
  year: number,
  signal: AbortSignal,
): Promise<StockChartResponse> {
  return fetchInventory(`/api/stock/pending/chart?year=${year}`, signal);
}

export async function fetchWarehousesPage(
  page: number,
  signal: AbortSignal,
): Promise<PaginatedResponse<Warehouse>> {
  return fetchInventory(
    `/api/stock/pending/warehouses?pageNumber=${page}&pageSize=${pageSize}`,
    signal,
  );
}

export function fetchArticlesPage(
  param: GetArticlesRequest,
  signal: AbortSignal,
): Promise<PaginatedResponse<ArticleDto>> {
  return fetchInventory(
    `/api/articles?articleName=${param.articleName}&pageNumber=${param.pageNumber}&pageSize=${param.pageSize}&sortKey=${param.sortKey}&sortDirection=${param.sortDirection}`,
    signal,
  );
}

function paginate<T>(data: T[], page: number): PaginatedResponse<T> {
  return {
    data: data.slice((page - 1) * pageSize, page * pageSize),
    totalCount: data.length,
    totalPages: Math.ceil(data.length / pageSize),
  };
}

async function fetchInventory<T>(url: string, signal: AbortSignal): Promise<T> {
  const response = await fetch(url, { signal });
  if (response.status !== 200) {
    throw new Error(`Failed to fetch data. (HTTP ${response.status}).`);
  }
  return response.json();
}

export class WarehouseNotFoundError extends Error {}
