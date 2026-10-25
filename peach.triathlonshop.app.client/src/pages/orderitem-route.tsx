import { useParams } from '@tanstack/react-router'
import OrderItemPage from './orderitem'

function OrderItemRoute() {
    const { id } = useParams({ from: '/orders/$id' })
    return <OrderItemPage id={id} />
}

export default OrderItemRoute
