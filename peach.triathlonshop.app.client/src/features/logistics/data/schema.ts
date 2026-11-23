export enum SortKey {
    Id = "id",
    OrderNumber = "orderNumber",
    TransportStatus = "transportStatus",
    EstimatedDeliveryDate = "estimatedDeliveryDate",
    PlacedAt = "placedAt",
    CreatedAt = "createdAt"
}

export enum ColumnId {
    Id = "id",
    Destination = "destination",
    OrderNumber = "orderNumber",
    TransportStatus = "transportStatus",
    EstimatedDeliveryDate = "estimatedDeliveryDate",
    PlacedAt = "placedAt",
    CreatedAt = "createdAt"
}

export type WarehouseResponseDto = {
    warehouses: WarehouseDto[];
    totalWarehouses: number;
};

export type WarehouseDto = {
    warehouseId: number;
    warehouseName: string;
    location: LocationDto;
    articles: ArticleDto[];
};

export type LocationDto = {
    countryCode: string;
    countryName?: string | null;
    cityName: string;
    latitude?: number | null;
    longitude?: number | null;
};

export type ArticleDto = {
    articleId: number;
    articleName: string;
    articleNumber: string;
    quantity: number;
};