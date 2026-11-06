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
  invoices: OrderInvoice[];
  transportOrders: OrderTransport[];
  notifications: OrderNotification[];
  createdAt: string;
};

export type OrderInvoice = {
  id: number;
  status: string;
  createdAt: string;
  updatedAt?: string | null;
};

export type OrderTransport = {
  id: number;
  status: string;
  createdAt: string;
  consignmentNumber?: string;
  trackingUrl?: string;
  carrier?: string;
  destination?: string;
  shippedAt?: string;
  estimatedDeliveryDate?: string;
  updatedAt?: string | null;
};

export type OrderNotification = {
  id: number;
  orderStatus: string;
  destination: string;
  createdAt: string;
  updatedAt?: string | null;
};

export type Location = {
  countryCode: string;
  countryName?: string;
  cityName: string;
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
