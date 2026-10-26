export interface DashboardData {
  totalRevenue: number
  totalCustomers: number
  totalOrders: number
  activeOrders: number
  monthlyRevenue: MonthlyRevenue[]
  recentOrdersLast30Days: RecentOrder[]
}

export interface MonthlyRevenue {
  month: number
  revenue: number
}

export interface RecentOrder {
  id: number
  orderNumber: string
  totalPrice: number
  createdAt: string
}
