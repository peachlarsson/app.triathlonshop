import { Alert, Box, Paper, Stack, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';

interface StatsState {
    totalOrders: number;
    totalOrderRows: number;
    totalArticles: number;
    totalCustomers: number;
}

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
        <Paper
            elevation={2}
            onClick={onClick}
            sx={{ p: 3, cursor: 'pointer' }}
        >
            <Typography variant="subtitle1" color="text.secondary">
                {label}
            </Typography>
            <Typography variant="h4">
                {value}
            </Typography>
        </Paper>
    );
}

function HomePage() {
    const navigate = useNavigate();

    const { data: stats, isLoading, isError } = useQuery({
        queryKey: ['statistics'],
        queryFn: fetchStatistics
    });

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