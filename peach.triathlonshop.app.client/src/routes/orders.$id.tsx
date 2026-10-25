import { createFileRoute } from '@tanstack/react-router'
import OrderItemRoute from '../pages/orderitem-route'

export const Route = createFileRoute('/orders/$id')({
    component: OrderItemRoute,
})
