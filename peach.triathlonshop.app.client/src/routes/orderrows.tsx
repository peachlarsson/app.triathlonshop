import { createFileRoute } from '@tanstack/react-router'
import OrderRowsPage from '../pages/orderrows'

export const Route = createFileRoute('/orderrows')({
    component: OrderRowsPage,
})
