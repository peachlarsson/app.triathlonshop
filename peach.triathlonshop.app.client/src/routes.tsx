import { Navigate, createBrowserRouter } from 'react-router-dom'
import App from './App'
import HomePage from './pages/home'
import OrderRowsPage from './pages/orderrows'
import OrdersPage from './pages/orders'
import ArticlesPage from './pages/articles'

export const router = createBrowserRouter(
    [{
        path: '/',
        element: <App />,
        children: [
            { index: true, element: <HomePage /> },
            { path: 'orderrows', element: <OrderRowsPage /> },
            { path: 'orders', element: <OrdersPage /> },
            { path: 'articles', element: <ArticlesPage /> },
            { path: '*', element: <Navigate to="/" replace /> },
        ]
    }]
);