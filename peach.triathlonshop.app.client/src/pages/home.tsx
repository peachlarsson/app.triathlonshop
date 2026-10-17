import { Alert, Box, Paper, Stack, Typography } from '@mui/material';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createSignalRConnection } from '../hooks/useSignalRConnection';

interface StatsState {
    totalOrders: number;
    totalOrderRows: number;
    totalArticles: number;
    totalCustomers: number;
}

interface SignalRHomePageMessage {
    Message?: string;
    Update?: boolean;
    Timestamp?: string;
}

const countEndpointByMessage: Record<string, keyof StatsState> = {
    Orders: 'totalOrders',
    OrderRows: 'totalOrderRows',
    Articles: 'totalArticles',
    Customers: 'totalCustomers'
};

const endpointByMessage: Record<string, string> = {
    Orders: '/api/orders',
    OrderRows: '/api/orderrows',
    Articles: '/api/articles',
    Customers: '/api/customers'
};

async function fetchTotalCount(endpoint: string): Promise<number> {
    const response = await fetch(endpoint);

    if (!response.ok) {
        throw new Error(`Request failed for ${endpoint}`);
    }

    const result = await response.json() as { totalCount?: number };
    return result.totalCount ?? 0;
}

async function fetchStatistics(): Promise<StatsState> {
    const [totalOrders, totalOrderRows, totalArticles, totalCustomers] = await Promise.all([
        fetchTotalCount('/api/orders'),
        fetchTotalCount('/api/orderrows'),
        fetchTotalCount('/api/articles'),
        fetchTotalCount('/api/customers')
    ]);

    return {
        totalOrders,
        totalOrderRows,
        totalArticles,
        totalCustomers
    };
}

function StatisticCard({ label, value, onClick }: { label: string; value: number; onClick: () => void }) {
    return (
        <Paper elevation={2} onClick={onClick} sx={{ p: 3, cursor: 'pointer' }}>
            <Typography variant="subtitle1" color="text.secondary">
                {label}
            </Typography>
            <Typography variant="h4">{value}</Typography>
        </Paper>
    );
}

function HomePage() {
    const navigate = useNavigate();
    const [stats, setStats] = useState<StatsState | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isError, setIsError] = useState(false);

    const refreshStatisticFromSignalRMessage = async (msg: string) => {
        if (!msg) {
            return;
        }

        const statKey = countEndpointByMessage[msg];
        const endpoint = endpointByMessage[msg];

        console.log(`refreshing statistics: ${msg}`);

        if (!statKey || !endpoint) {
            return;
        }

        const totalCount = await fetchTotalCount(endpoint);

        console.log(`fetched total count for ${msg}: ${totalCount}`);

        setStats((current) => {
            if (!current) {
                return current;
            }

            return {
                ...current,
                [statKey]: totalCount
            };
        });
    };

    useEffect(() => {
        let isDisposed = false;
        let signalRconnection: Awaited<ReturnType<typeof createSignalRConnection>> | null = null;

        const connect = async () => {
            try {
                const initialStats = await fetchStatistics();

                console.log("Fetch initial success");

                if (!isDisposed) {
                    setStats(initialStats);
                    setIsLoading(false);
                }

                console.log("Connecting to SignalR...");

                signalRconnection = await createSignalRConnection('homepage');

                console.log("SignalR connection established");

                signalRconnection.on('homepage', (msg: SignalRHomePageMessage) => {
                    void refreshStatisticFromSignalRMessage(msg.Message);
                });

                await signalRconnection.start();

                if (isDisposed) {
                    await signalRconnection.stop();
                }
            } catch {
                if (!isDisposed) {
                    setIsLoading(false);
                }
            }
        };

        void connect();

        return () => {
            isDisposed = true;
            void signalRconnection?.stop();
        };
    }, []);

    return (
        <Stack spacing={3}>
            <Box>
                <Typography variant="h4" gutterBottom>
                    peach.triathlonshop.com: Hello world!
                </Typography>
            </Box>

            {isError && <Alert severity="error">Failed to load.</Alert>}

            {isLoading || !stats ? (
                <Typography variant="body1">Loading...</Typography>
            ) : (
                <Stack spacing={2}>
                    <StatisticCard label="Orders" value={stats.totalOrders} onClick={() => navigate('/orders')} />
                    <StatisticCard label="Order rows" value={stats.totalOrderRows} onClick={() => navigate('/orderrows')} />
                    <StatisticCard label="Articles" value={stats.totalArticles} onClick={() => navigate('/articles')} />
                    <StatisticCard label="Customers" value={stats.totalCustomers} onClick={() => navigate('/customers')} />
                </Stack>
            )}
        </Stack>
    );
}

export default HomePage;