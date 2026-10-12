import { Navigate, createBrowserRouter } from 'react-router-dom'
import App from './App'
import HomePage from './pages/home'
import OrderRowsPage from './pages/orderrows'
import OrdersPage from './pages/orders'
import OrderItemPage from './pages/orderitem'
import ArticlesPage from './pages/articles'
import CustomersPage from './pages/customers'

export const router = createBrowserRouter(
    [{
        path: '/',
        element: <App />,
        children: [
            { index: true, element: <HomePage /> },
            { path: 'orderrows', element: <OrderRowsPage /> },
            { path: 'orders', element: <OrdersPage /> },
            { path: 'orders/:id', element: <OrderItemPage /> },
            { path: 'articles', element: <ArticlesPage /> },
            { path: 'customers', element: <CustomersPage /> },
            { path: '*', element: <Navigate to="/" replace /> },
        ]
    }]
);