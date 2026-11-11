import type {
  ArticleDto,
  ArticleWarehouseStock,
  PaginatedResponse,
  StockItem,
  StockOverviewResponse,
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
    const stockWarehouse = stock.find(
      (item) => item.warehouseId === warehouseId,
    );
    const warehouse: Warehouse = {
      id: warehouseId,
      warehouseName: stockWarehouse?.warehouseName ?? `Warehouse ${warehouseId}`,
      city: stockWarehouse?.location.cityName ?? null,
      countryName: stockWarehouse?.location.countryCode ?? null,
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
    }
  }
  return { warehouses, articlesByWarehouse, stockItems };
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
): Promise<StockOverviewResponse> {
  return fetchInventory<StockOverviewResponse>("/api/stock/overview", signal);
}

export async function fetchStockPage(
  page: number,
  signal: AbortSignal,
): Promise<PaginatedResponse<StockItem>> {
  return paginate((await loadInventory(signal)).stockItems, page);
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

export async function fetchArticlesPage(
  param: GetArticlesRequest,
  signal: AbortSignal,
): Promise<PaginatedResponse<ArticleDto>> {
  return fetchInventory(
    `/api/articles?articleName=${param.articleName ?? ""}&pageNumber=${param.pageNumber}&pageSize=${param.pageSize}&sortKey=${param.sortKey}&sortDirection=${param.sortDirection}`,
    signal,
  );
}

export async function fetchArticleWarehouseStock(
  signal: AbortSignal,
): Promise<ArticleWarehouseStock> {
  const stock = await fetchInventory<WarehouseStockDto[]>(
    "/api/stock/all-warehouses",
    signal,
  );
  return {
    warehouses: stock.map(({ warehouseId, warehouseName }) => ({
      id: warehouseId,
      name: warehouseName,
    })),
    quantities: stock.flatMap(({ warehouseId, articles }) =>
      articles.map(({ articleId, quantity }) => ({
        articleId,
        warehouseId,
        quantity,
      })),
    ),
  };
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
