import { Box, Button, Stack, Typography } from '@mui/material';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { DataTable, type ColumnDefinition } from '../components/DataTable';

interface OrderRow {
    id: number;
    orderId: number;
    orderNumber: string;
    lineNumber: number;
    articleId: number;
    articleName: string;
    unitPrice: number;
    quantity: number;
    rowPrice: number;
    customerId: number;
    customerName: string;
    createdAt: string;
    updatedAt?: string | null;
}

interface Order {
    id: number;
    number: string;
    items?: OrderRow[];
    totalPrice: number;
    isConfirmed: boolean;
    customerId: number;
    customerName: string;
    createdAt: string;
    updatedAt?: string | null;
}

function App() {

    const { id } = useParams();
    const [order, setOrder] = useState<Order | null>(null);
    const [sortBy, setSortBy] = useState('Id');
    const [sortOrder, setSortOrder] = useState('Asc');

    const orderItemColumns: ColumnDefinition[] = [
        { key: 'lineNumber', label: 'Line Number' },
        { key: 'articleName', label: 'Article Name' },
        { key: 'unitPrice', label: 'Unit Price', isPrice: true },
        { key: 'quantity', label: 'Quantity' },
        { key: 'rowPrice', label: 'Row Price', isPrice: true },
        { key: 'customerId', label: 'Customer Id' },
        { key: 'customerName', label: 'Customer Name' }
    ];

    useEffect(() => {
        if (!id) return;

        fetch(`/api/orders/${id}?sortBy=${sortBy}&sortOrder=${sortOrder}`)
            .then(r => r.json())
            .then(setOrder);
    }, [id, sortBy, sortOrder]);

    if (!order) {
        return <Typography>Loading...</Typography>;
    }

    return (
        <Stack spacing={4}>
            <Box>
                <Typography variant="h4" gutterBottom>
                    Order #{order.id}
                </Typography>

                <Typography>Order number: {order.number}</Typography>
                <Typography>Customer: {order.customerName}</Typography>
                <Typography>Total price: SEK {order.totalPrice}</Typography>

                <Typography sx={{ mt: 2, mb: 1 }}>Items:</Typography>
                <DataTable
                    columns={orderItemColumns}
                    data={order.items ?? []}
                    isLoading={!order}
                    tableId="orderItemTableLabel"
                    sortBy={sortBy}
                    sortOrder={sortOrder}
                    onSortChange={(sortBy, sortOrder) => {
                        setSortBy(sortBy);
                        setSortOrder(sortOrder);
                    }}
                />

                {!order.isConfirmed && (
                    <Button sx={{ mt: 2, float: 'right' }}

                        variant="contained"
                        onClick={() => {
                            // Call the API /orders/{id}/confirm
                        }}
                    >
                        Confirm Order
                    </Button>
                )}

                {order.isConfirmed && (
                    <Typography sx={{ mt: 2, float: 'right' }}>
                        Order is confirmed
                    </Typography>
                )}
            </Box>
        </Stack>
    );
}

export default App;