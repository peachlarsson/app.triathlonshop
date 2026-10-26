import { Link } from '@tanstack/react-router'
import type { RecentOrder } from '../data/schema'

interface RecentSalesProps {
  orders: RecentOrder[]
}

export function RecentSales({ orders }: RecentSalesProps) {
  if (orders.length === 0) {
    return (
      <div className='py-8 text-center text-sm text-muted-foreground'>
        No orders in the last 30 days.
      </div>
    )
  }

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('sv-SE', {
      style: 'currency',
      currency: 'SEK',
    }).format(value)

  return (
    <div className='max-h-[350px] space-y-4 overflow-auto'>
      {orders.map((order) => (
        <Link
          key={order.orderNumber}
          to='/orders/$orderId'
          params={{ orderId: String(order.id) }}
          className='flex items-center justify-between gap-4 rounded-md focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring'
        >
          <div className='min-w-0'>
            <p className='truncate text-sm font-medium hover:underline'>
              Order #{order.orderNumber}
            </p>
            <p className='text-xs text-muted-foreground'>
              {new Intl.DateTimeFormat('en-US', {
                dateStyle: 'medium',
              }).format(new Date(order.createdAt))}
            </p>
          </div>
          <p className='shrink-0 text-sm font-medium'>
            {formatCurrency(order.totalPrice)}
          </p>
        </Link>
      ))}
    </div>
  )
}
