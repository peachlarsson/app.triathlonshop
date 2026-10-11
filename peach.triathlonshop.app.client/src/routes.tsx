import { Navigate, createBrowserRouter } from 'react-router-dom'
import App from './App'
import OrderRowsPage from './pages/orderrows'
import ArticlesPage from './pages/articles'

export const router = createBrowserRouter(
    [{
        path: '/',
        element: <App />,
        children: [
            { index: true, element: <Navigate to="/orderrows" replace /> },
            { path: 'orderrows', element: <OrderRowsPage /> },
            { path: 'articles', element: <ArticlesPage /> },
            { path: '*', element: <Navigate to="/orderrows" replace /> },
        ]
    }]
);