import { Alert, Box, Button, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import { useEffect, useMemo, useState } from 'react';

interface Customer {
    id: number;
    name: string | null;
    email: string | null;
}

interface Article {
    id: number;
    name: string;
    unitPrice: number;
}

interface CreateOrderRow {
    articleId: number;
    quantity: number;
    lineNumber: number;
}

interface CreateOrderPayload {
    orderRows: CreateOrderRow[];
    customerId: number;
}

interface FormOrderRow {
    articleId: string;
    quantity: string;
}

interface PaginatedApiResponse<T> {
    data: T[];
}

interface AddOrderCardProps {
    onOrderCreated: () => Promise<void>;
}

const getResponseData = <T,>(result: PaginatedApiResponse<T> | T[]) =>
    Array.isArray(result) ? result : result.data;

const getDefaultOrderRow = (articleList: Article[]): FormOrderRow => ({
    articleId: articleList.length > 0 ? articleList[0].id.toString() : '',
    quantity: '1'
});

function AddOrderCard({ onOrderCreated }: AddOrderCardProps) {
    const [customers, setCustomers] = useState<Customer[]>([]);
    const [articles, setArticles] = useState<Article[]>([]);
    const [rows, setRows] = useState<FormOrderRow[]>([]);
    const [customerId, setCustomerId] = useState('');
    const [formError, setFormError] = useState<string | null>(null);
    const [formSuccess, setFormSuccess] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isVisible, setIsVisible] = useState(false);

    const setFormMessage = (message: string | null) => {
        setFormError(message);
        setFormSuccess(null);
    };

    useEffect(() => {
        const loadOptions = async () => {
            try {
                const [customersResponse, articlesResponse] = await Promise.all([
                    fetch('/api/customers?pageSize=1000'),
                    fetch('/api/articles?pageSize=1000&sortby=name')
                ]);

                if (!customersResponse.ok || !articlesResponse.ok)
                    throw new Error('Failed to load order create options.');

                const customersResult = await customersResponse.json() as PaginatedApiResponse<Customer> | Customer[];
                const articlesResult = await articlesResponse.json() as PaginatedApiResponse<Article> | Article[];

                const loadedCustomers = getResponseData(customersResult);
                const loadedArticles = getResponseData(articlesResult);

                setCustomers(loadedCustomers);
                setArticles(loadedArticles);
                setRows([getDefaultOrderRow(loadedArticles)]);

                if (loadedCustomers.length > 0)
                    setCustomerId(loadedCustomers[0].id.toString());
            } catch {
                setFormMessage('Failed to load customers and articles.');
            }
        };

        void loadOptions();
    }, []);

    const rowPriceValues = useMemo(
        () => rows.map((row) => {
            const article = articles.find((x) => x.id.toString() === row.articleId);
            const quantity = Number(row.quantity);

            if (!article || Number.isNaN(quantity) || quantity <= 0)
                return 0;

            return article.unitPrice * quantity;
        }),
        [rows, articles]
    );

    const totalPrice = useMemo(
        () => rowPriceValues.reduce((sum, value) => sum + value, 0).toFixed(2),
        [rowPriceValues]
    );

    const addRow = () => setRows((current) => [...current, getDefaultOrderRow(articles)]);

    const removeRow = (rowIndex: number) => {
        setRows((current) => current.filter((_, index) => index !== rowIndex));
    };

    const updateRow = (rowIndex: number, key: keyof FormOrderRow, value: string) => {
        setRows((current) =>
            current.map((row, index) => (index === rowIndex ? { ...row, [key]: value } : row))
        );
    };

    const handleCreateOrder = async () => {
        const parsedCustomerId = Number(customerId);

        if (Number.isNaN(parsedCustomerId) || parsedCustomerId <= 0) {
            setFormMessage('Customer is required.');
            return;
        }

        if (rows.length === 0) {
            setFormMessage('At least one order row is required.');
            return;
        }

        const orderRows: CreateOrderRow[] = rows.map((row, index) => ({
            articleId: Number(row.articleId),
            quantity: Number(row.quantity),
            lineNumber: index + 1
        }));

        const invalidArticleRow = orderRows.some((row) => Number.isNaN(row.articleId) || row.articleId <= 0);
        const invalidQuantityRow = orderRows.some((row) => Number.isNaN(row.quantity) || row.quantity <= 0 || !Number.isInteger(row.quantity));

        if (invalidArticleRow) {
            setFormMessage('Choose an article for each row.');
            return;
        }

        if (invalidQuantityRow) {
            setFormMessage('Quantity must be a whole number greater than 0 for every row.');
            return;
        }

        const payload: CreateOrderPayload = {
            orderRows,
            customerId: parsedCustomerId
        };

        setIsSubmitting(true);
        setFormError(null);
        setFormSuccess(null);

        try {
            const response = await fetch('/api/orders', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || 'Failed to create order.');
            }

            setFormSuccess('Order created successfully.');
            setRows([getDefaultOrderRow(articles)]);
            await onOrderCreated();
        } catch (error) {
            const message = error instanceof Error ? error.message : 'Failed to create order.';
            setFormMessage(message);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <Paper elevation={2} sx={{ p: 2, mb: 2, position: 'relative' }}>
            <Button
                variant="outlined"
                onClick={() => setIsVisible((current) => !current)}
                sx={{ position: 'absolute', top: 16, right: 16 }}
            >
                {isVisible ? 'Hide' : 'Show'}
            </Button>

            <Stack spacing={2}>
                <Typography variant="h6">New Order</Typography>

                {isVisible && (
                    <>
                        {formError && <Alert severity="error">{formError}</Alert>}
                        {formSuccess && <Alert severity="success">{formSuccess}</Alert>}

                        <TextField
                            select
                            fullWidth
                            label="Customer"
                            value={customerId}
                            onChange={(event) => setCustomerId(event.target.value)}
                        >
                            {customers.map((customer) => (
                                <MenuItem key={customer.id} value={customer.id.toString()}>
                                    {customer.name ?? `Customer ${customer.id}`} - {customer.email}
                                </MenuItem>
                            ))}
                        </TextField>

                        {rows.map((row, index) => (
                            <Stack key={`order-row-${index}`} direction={{ xs: 'column', md: 'row' }} spacing={2}>
                                <TextField
                                    select
                                    fullWidth
                                    label="Choose article"
                                    value={row.articleId}
                                    onChange={(event) => updateRow(index, 'articleId', event.target.value)}
                                >
                                    {articles.map((article) => (
                                        <MenuItem key={article.id} value={article.id.toString()}>
                                            {article.name} - SEK {article.unitPrice.toFixed(2)}
                                        </MenuItem>
                                    ))}
                                </TextField>

                                <TextField
                                    fullWidth
                                    label="Quantity"
                                    type="number"
                                    value={row.quantity}
                                    onChange={(event) => updateRow(index, 'quantity', event.target.value)}
                                    slotProps={{ htmlInput: { min: 1, step: 1 } }}
                                />

                                <TextField
                                    fullWidth
                                    label="Price (SEK)"
                                    value={rowPriceValues[index].toFixed(2)}
                                    slotProps={{ input: { readOnly: true } }}
                                />

                                <Button
                                    variant="text"
                                    color="inherit"
                                    size="small"
                                    onClick={() => removeRow(index)}
                                    disabled={rows.length <= 1 || isSubmitting}
                                >
                                    Remove
                                </Button>
                            </Stack>
                        ))}

                        <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                            <TextField
                                label="Total Price (SEK)"
                                value={totalPrice}
                                slotProps={{ input: { readOnly: true } }}
                                sx={{ minWidth: 220 }}
                            />
                        </Box>

                        <Button
                            variant="outlined"
                            onClick={addRow}
                            disabled={isSubmitting || articles.length === 0}
                            sx={{ alignSelf: 'flex-start' }}
                        >
                            +
                        </Button>

                        <Button
                            variant="contained"
                            onClick={() => void handleCreateOrder()}
                            disabled={isSubmitting || customers.length === 0 || articles.length === 0}
                        >
                            {isSubmitting ? 'Adding...' : 'Add Order'}
                        </Button>
                    </>
                )}
            </Stack>
        </Paper>
    );
}

export default AddOrderCard;
