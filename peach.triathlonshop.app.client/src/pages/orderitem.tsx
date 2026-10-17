import { Box, Button, Stack, Typography } from '@mui/material';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { DataTable, type ColumnDefinition } from '../components/DataTable';
import { TimelineSection } from '../components/TimelineSection';
import AddOrderItemCard from '../components/AddOrderRowCard';

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
    status: string;
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

interface SignalRNegotiateResponse {
    url: string;
    accessToken: string;
}

interface SignalRTestMessagePayload {
    Message?: unknown;
    [key: string]: unknown;
}

interface SignalRMessageTimelineItem {
    id: number;
    message: string;
    receivedAt: string;
}

function App() {

    const { id } = useParams();
    const [order, setOrder] = useState<Order | null>(null);
    const [signalRMessages, setSignalRMessages] = useState<SignalRMessageTimelineItem[]>([]);
    const isConfirmed = order?.status === 'Confirmed';

    const fetchOrder = async (): Promise<Order | null> => {
        if (!id) {
            return null;
        }

        console.log(`Fetching order: /api/orders/${id}`);

        const response = await fetch(`/api/orders/${id}`);
        const responseData = await response.json();
        return responseData as Order;
    };

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

        let ignore = false;

        const loadOrder = async () => {
            const responseData = await fetchOrder();

            if (!ignore && responseData) {
                setOrder(responseData);
            }
        };

        void loadOrder();

        return () => {
            ignore = true;
        };
    }, [id]);

    useEffect(() => {
        let isDisposed = false;

        const connectSignalR = async () => {
            try {
                const negotiateResponse = await fetch('/api/orders/negotiation');

                if (!negotiateResponse.ok) {
                    throw new Error('Failed to negotiate SignalR connection.');
                }

                const { url, accessToken } = await negotiateResponse.json() as SignalRNegotiateResponse;

                const connection = new HubConnectionBuilder()
                    .withUrl(url, {
                        accessTokenFactory: () => accessToken
                    })
                    .build();

                connection.on(`order${id}`, async (msg: SignalRTestMessagePayload) => {
                    const timestamp = new Date().toISOString();
                    const message = typeof msg?.Message === 'string'
                        ? msg.Message
                        : JSON.stringify(msg);

                    setSignalRMessages((current) => [
                        {
                            id: Date.now() + current.length,
                            message,
                            receivedAt: timestamp
                        },
                        ...current
                    ]);

                    const responseData = await fetchOrder();
                    if (responseData) {
                        setOrder(responseData);
                    }
                });

                await connection.start();

                if (isDisposed) {
                    await connection.stop();
                    return;
                }

                window.addEventListener('beforeunload', () => {
                    void connection.stop();
                }, { once: true });

                return connection;
            } catch (error) {
                console.error('SignalR connection error', error);
            }
        };

        const activeConnectionPromise = connectSignalR();

        return () => {
            isDisposed = true;
            void activeConnectionPromise.then((connection) => connection?.stop());
        };
    }, [id]);

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
                <Typography>Status: {order.status}</Typography>

                {isConfirmed && (
                    <Typography sx={{ mt: 2 }}>[ CONFIRMED ]</Typography>
                )}

                {!isConfirmed && (
                    <Button
                        sx={{ mt: 2, float: 'right' }}
                        variant="contained"
                        onClick={async () => {
                            if (!id) {
                                return;
                            }

                            const response = await fetch(`/api/orders/confirm/${id}`, {
                                method: 'POST'
                            });

                            if (response.ok) {
                                const refreshedOrderData = await fetchOrder();
                                if (refreshedOrderData) {
                                    setOrder(refreshedOrderData);
                                }
                            }
                        }}
                    >
                        Confirm Order
                    </Button>
                )}

                <Stack spacing={3} sx={{ mt: 3 }}>
                    <Box sx={{ p: 2 }}>
                        <Stack spacing={2}>
                            <Typography variant="h6">Items:</Typography>
                            <DataTable
                                columns={orderItemColumns}
                                data={order.items ?? []}
                                isLoading={!order}
                                tableId="orderItemTableLabel"
                            />
                        </Stack>
                    </Box>

                    {!isConfirmed && id && (
                        <AddOrderItemCard
                            orderId={Number(id)}
                            onOrderItemCreated={async () => {
                                const responseData = await fetchOrder();
                                if (responseData) {
                                    setOrder(responseData);
                                }
                            }}
                        />
                    )}

                    <Box
                        sx={{
                            display: 'grid',
                            gridTemplateColumns: { xs: '1fr', md: '2fr 1fr' },
                            gap: 3,
                            alignItems: 'start'
                        }}
                    >
                        <Stack spacing={3}>
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

                        <TimelineSection
                            title="SignalR Messages"
                            items={signalRMessages}
                            emptyText=""
                            getKey={(item) => item.id}
                            getTitle={(item) => item.message}
                            getTimestamp={(item) => item.receivedAt}
                            renderDetails={(item) => (
                                <Typography variant="body2">Received at: {item.receivedAt}</Typography>
                            )}
                        />
                    </Box>
                </Stack>
            </Box>
        </Stack>
    );
}

export default App;