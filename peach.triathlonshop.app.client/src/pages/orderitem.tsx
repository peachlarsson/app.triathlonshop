import { Box, Button, Stack, Typography } from '@mui/material';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { DataTable, type ColumnDefinition } from '../components/DataTable';
import { TimelineSection } from '../components/TimelineSection';

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

interface NotificationHistoryItem {
    id: number;
    orderStatus: string;
    destination: string;
    createdAt: string;
    updatedAt?: string | null;
}

interface TransportHistoryItem {
    id: number;
    status: string;
    carrier?: string | null;
    consignmentNumber?: string | null;
    trackingUrl?: string | null;
    destination?: string | null;
    shippedAt?: string | null;
    estimatedDeliveryDate?: string | null;
    createdAt: string;
    updatedAt?: string | null;
}

interface InvoiceHistoryItem {
    id: number;
    status: string;
    createdAt: string;
    updatedAt?: string | null;
}

interface Order {
    id: number;
    number: string;
    items?: OrderRow[];
    notificationHistory?: NotificationHistoryItem[];
    transportHistory?: TransportHistoryItem[];
    invoiceHistory?: InvoiceHistoryItem[];
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

                <Stack spacing={3} sx={{ mt: 3 }}>
                    <Box sx={{ p: 2 }}>
                        <Stack spacing={2}>
                            <Typography variant="h6">Items:</Typography>
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
                        </Stack>
                    </Box>

                    <TimelineSection
                        title="Invoice"
                        items={order.invoiceHistory ?? []}
                        emptyText="No invoice updates."
                        getKey={(item) => item.id}
                        getTitle={(item) => item.status}
                        getTimestamp={(item) => item.createdAt}
                    />

                    <TimelineSection
                        title="Transport"
                        items={order.transportHistory ?? []}
                        emptyText="No transport updates."
                        getKey={(item) => item.id}
                        getTitle={(item) => item.status}
                        getTimestamp={(item) => item.createdAt}
                        renderDetails={(item) => (
                            <>
                                {item.carrier && <Typography variant="body2">Carrier: {item.carrier}</Typography>}
                                {item.consignmentNumber && <Typography variant="body2">Consignment: {item.consignmentNumber}</Typography>}
                                {item.destination && <Typography variant="body2">Destination: {item.destination}</Typography>}
                                {item.trackingUrl && <Typography variant="body2">Tracking: {item.trackingUrl}</Typography>}
                            </>
                        )}
                    />

                    <TimelineSection
                        title="Notifications"
                        items={order.notificationHistory ?? []}
                        emptyText="No notifications sent."
                        getKey={(item) => item.id}
                        getTitle={(item) => item.orderStatus}
                        getTimestamp={(item) => item.createdAt}
                        renderDetails={(item) => (
                            <Typography variant="body2">Sent to: {item.destination}</Typography>
                        )}
                    />
                </Stack>

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