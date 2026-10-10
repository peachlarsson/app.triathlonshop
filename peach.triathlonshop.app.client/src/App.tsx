import { useEffect, useState } from 'react';
import {
    Box,
    CircularProgress,
    Container,
    Paper,
    Stack,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Typography
} from '@mui/material';

interface Article {
    id: number;
    name: string;
    unitPrice: number;
}

interface Customer {
    id: number;
    isGuest: boolean;
    name: string;
    email: string;
}

interface Order {
    id: number;
    customerId: number;
    orderNumber: string;
    totalPrice: number;
    orderRows: OrderRow[];
}

interface OrderRow {
    id: number;
    orderId: number;
    orderNumber: string;
    lineNumber: number;
    articleId: number;
    articleName: string;
    unitPrice: number;
    quantity: number;
}

function App() {
    const [articles, setArticles] = useState<Article[]>();
    const [customers, setCustomers] = useState<Customer[]>();
    const [orders, setOrders] = useState<Order[]>();
    const [orderRows, setOrderRows] = useState<OrderRow[]>();

    useEffect(() => {
        getArticles();
        getCustomers();
        getOrders();
        getOrderRows();
    }, []);

    const renderLoading = (text: string) => (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <CircularProgress size={18} />
            <Typography variant="body2">{text}</Typography>
        </Stack>
    );

    const renderNoData = (text: string) => (
        <Typography variant="body2" color="text.secondary">
            {text}
        </Typography>
    );

    const articleContents = articles === undefined
        ? renderLoading('Loading articles...')
        : articles.length === 0
            ? renderNoData('No articles found.')
            : (
                <TableContainer component={Paper} variant="outlined">
                    <Table size="small" aria-labelledby="articlesTableLabel">
                        <TableHead>
                            <TableRow>
                                <TableCell>Id</TableCell>
                                <TableCell>Name</TableCell>
                                <TableCell align="right">Price</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {articles.map((article) => (
                                <TableRow key={article.id} hover>
                                    <TableCell>{article.id}</TableCell>
                                    <TableCell>{article.name}</TableCell>
                                    <TableCell align="right">{article.unitPrice.toFixed(2)}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            );

    const customerContents = customers === undefined
        ? renderLoading('Loading customers...')
        : customers.length === 0
            ? renderNoData('No customers found.')
            : (
                <TableContainer component={Paper} variant="outlined">
                    <Table size="small" aria-labelledby="customersTableLabel">
                        <TableHead>
                            <TableRow>
                                <TableCell>Id</TableCell>
                                <TableCell>Guest Customer</TableCell>
                                <TableCell>Name</TableCell>
                                <TableCell>Email</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {customers.map((customer) => (
                                <TableRow key={customer.id} hover>
                                    <TableCell>{customer.id}</TableCell>
                                    <TableCell>{customer.isGuest ? 'Yes' : 'No'}</TableCell>
                                    <TableCell>{customer.name}</TableCell>
                                    <TableCell>{customer.email}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            );

    const orderContents = orders === undefined
        ? renderLoading('Loading orders...')
        : orders.length === 0
            ? renderNoData('No orders found.')
            : (
                <TableContainer component={Paper} variant="outlined">
                    <Table size="small" aria-labelledby="ordersTableLabel">
                        <TableHead>
                            <TableRow>
                                <TableCell>Id</TableCell>
                                <TableCell>CustomerId</TableCell>
                                <TableCell>OrderNumber</TableCell>
                                <TableCell align="right">TotalPrice</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {orders.map((order) => (
                                <TableRow key={order.id} hover>
                                    <TableCell>{order.id}</TableCell>
                                    <TableCell>{order.customerId}</TableCell>
                                    <TableCell>{order.orderNumber}</TableCell>
                                    <TableCell align="right">{order.totalPrice.toFixed(2)}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            );

    const orderRowContents = orderRows === undefined
        ? renderLoading('Loading order rows...')
        : orderRows.length === 0
            ? renderNoData('No order rows found.')
            : (
                <TableContainer component={Paper} variant="outlined">
                    <Table size="small" aria-labelledby="orderRowsTableLabel">
                        <TableHead>
                            <TableRow>
                                <TableCell>Id</TableCell>
                                <TableCell>OrderId</TableCell>
                                <TableCell>OrderNumber</TableCell>
                                <TableCell>LineNumber</TableCell>
                                <TableCell>ArticleId</TableCell>
                                <TableCell>ArticleName</TableCell>
                                <TableCell align="right">UnitPrice</TableCell>
                                <TableCell align="right">Quantity</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {orderRows.map((orderRow) => (
                                <TableRow key={orderRow.id} hover>
                                    <TableCell>{orderRow.id}</TableCell>
                                    <TableCell>{orderRow.orderId}</TableCell>
                                    <TableCell>{orderRow.orderNumber}</TableCell>
                                    <TableCell>{orderRow.lineNumber}</TableCell>
                                    <TableCell>{orderRow.articleId}</TableCell>
                                    <TableCell>{orderRow.articleName}</TableCell>
                                    <TableCell align="right">{orderRow.unitPrice.toFixed(2)}</TableCell>
                                    <TableCell align="right">{orderRow.quantity}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            );

    return (
        <Container maxWidth="lg" sx={{ py: 4 }}>
            <Stack spacing={4}>
                <Box>
                    <Typography id="articlesTableLabel" variant="h4" gutterBottom>
                        Articles
                    </Typography>
                    {articleContents}
                </Box>

                <Box>
                    <Typography id="customersTableLabel" variant="h4" gutterBottom>
                        Customers
                    </Typography>
                    {customerContents}
                </Box>

                <Box>
                    <Typography id="ordersTableLabel" variant="h4" gutterBottom>
                        Orders
                    </Typography>
                    {orderContents}
                </Box>

                <Box>
                    <Typography id="orderRowsTableLabel" variant="h4" gutterBottom>
                        Order Rows
                    </Typography>
                    {orderRowContents}
                </Box>
            </Stack>
        </Container>
    );

    async function getArticles() {
        const response = await fetch('/article');

        if (response.ok) {
            const data: Article[] = await response.json();
            setArticles(data);
        } else {
            console.error('Failed to fetch articles:', response.status, response.statusText);
        }
    }

    async function getCustomers() {
        const response = await fetch('/customer');

        if (response.ok) {
            const data: Customer[] = await response.json();
            setCustomers(data);
        } else {
            console.error('Failed to fetch customers:', response.status, response.statusText);
        }
    }

    async function getOrders() {
        const response = await fetch('/order');

        if (response.ok) {
            const data: Order[] = await response.json();
            setOrders(data);
        } else {
            console.error('Failed to fetch orders:', response.status, response.statusText);
        }
    }

    async function getOrderRows() {
        const response = await fetch('/orderrow');

        if (response.ok) {
            const data: OrderRow[] = await response.json();
            setOrderRows(data);
        } else {
            console.error('Failed to fetch order rows:', response.status, response.statusText);
        }
    }
}

export default App;
