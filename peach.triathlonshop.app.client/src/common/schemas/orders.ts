export type Order = {
  id: number;
  orderNumber: string;
  status: OrderStatus;
  estimatedDelivery?: string;
  totalPrice: number;
  orderRows: OrderRow[];
  orderRowsCount?: number;
  customer: Customer;
  destinationLocation: Location;
  destinationPostalCode?: string;
  destinationAddress?: string;
  invoices: OrderInvoice[];
  transportOrder?: OrderTransport;
  transportOrders: OrderTransport[];
  notifications: OrderNotification[];
  placedAt: string;
  createdAt: string;
};

export enum OrderStatus {
  Created,
  Confirmed,
  Dispatched,
  Shipped,
  Cancelled,
}

export type OrderInvoice = {
  id: number;
  status: string;
  createdAt: string;
  updatedAt?: string | null;
};

export type OrderTransport = {
  id: number;
  transportOrderStatus: string;
  createdAt: string;
  consignmentNumber?: string;
  trackingUrl?: string;
  carrier?: string;
  destination?: string;
  dispatchDateTime?: string;
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
  placedAt: string;
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
