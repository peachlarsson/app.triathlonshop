import { Alert, Box, Paper, Stack, Typography } from '@mui/material';
import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

interface StatsState {
    totalOrders: number;
    totalArticles: number;
    totalCustomers: number;
}

const initialStats: StatsState = {
    totalOrders: 0,
    totalArticles: 0,
    totalCustomers: 0
};

async function fetchTotalCount(endpoint: string): Promise<number>
{
    const response = await fetch(`${endpoint}`);

    if (!response.ok) {
        throw new Error(`Request failed for ${endpoint}`);
    }

    const result = await response.json() as { totalCount?: number };
    return result.totalCount ?? 0;
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
    const [stats, setStats] = useState<StatsState>(initialStats);
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const navigate = useNavigate();

    const loadStatistics = useCallback(async () => {
        try {
            const [totalOrders, totalArticles, totalCustomers] = await Promise.all([
                fetchTotalCount('/api/orders'),
                fetchTotalCount('/api/articles'),
                fetchTotalCount('/api/customers')
            ]);

            setErrorMessage(null);
            setStats({
                totalOrders,
                totalArticles,
                totalCustomers
            });
        } catch {
            setErrorMessage('Failed to load statistics.');
        } finally {
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        const initialLoadTimeoutId = window.setTimeout(() => {
            void loadStatistics();
        }, 0);

        const intervalId = window.setInterval(() => {
            void loadStatistics();
        }, 15000);

        return () => {
            window.clearTimeout(initialLoadTimeoutId);
            window.clearInterval(intervalId);
        };
    }, [loadStatistics]);

    return (
        <Stack spacing={3}>
            <Box>
                <Typography variant="h4" gutterBottom>
                    peach.triathlonshop.com: Hello world!
                </Typography>
            </Box>

            {errorMessage && <Alert severity="error">{errorMessage}</Alert>}

            {isLoading ? (
                <Typography variant="body1">Loading statistics...</Typography>
            ) : (
                <Stack spacing={2}>
                    <StatisticCard label="Orders" value={stats.totalOrders} onClick={() => navigate('/orders')} />
                    <StatisticCard label="Articles" value={stats.totalArticles} onClick={() => navigate('/articles')} />
                    <StatisticCard label="Customers" value={stats.totalCustomers} onClick={() => navigate('/customers')} />
                </Stack>
            )}
        </Stack>
    );
}

export default HomePage;
