import { PaginatedResponse } from "@/common/schemas/paginated-response";
import { SortDirection } from "@/common/schemas/sort";

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

// Create order

export type CreateOrderRequestDto = {
    orderRows: CreateOrderRowDto[];
    customerId: number;
}

export type CreateOrderRowDto = {
    articleId: number;
    quantity: number;
}

// Create order form data

export type CreateOrderFormData = {
    orderRows: CreateOrderRowFormData[];
    customerId: number;
}

export type CreateOrderRowFormData = {
    articleId: number;
    articleName: string;
    quantity: string;
}