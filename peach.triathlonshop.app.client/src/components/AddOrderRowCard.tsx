import { Alert, Box, Button, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import { useEffect, useMemo, useState } from 'react';

interface Article {
    id: number;
    name: string;
    unitPrice: number;
}

interface ArticleCreateDto {
    id: number;
    quantity: number;
}

interface OrderRowCreateDto {
    articles: ArticleCreateDto[];
}

interface FormArticleRow {
    articleId: string;
    quantity: string;
}

interface PaginatedApiResponse<T> {
    data: T[];
}

interface AddOrderItemCardProps {
    orderId: number;
    onOrderItemCreated: () => Promise<void>;
}

const getResponseData = <T,>(result: PaginatedApiResponse<T> | T[]) =>
    Array.isArray(result) ? result : result.data;

const getDefaultArticleRow = (articleList: Article[]): FormArticleRow => ({
    articleId: articleList.length > 0 ? articleList[0].id.toString() : '',
    quantity: '1'
});

function AddOrderItemCard({ orderId, onOrderItemCreated }: AddOrderItemCardProps) {
    const [articles, setArticles] = useState<Article[]>([]);
    const [rows, setRows] = useState<FormArticleRow[]>([]);
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
                const response = await fetch('/api/articles?pageSize=1000&sortby=name');

                if (!response.ok)
                    throw new Error('Failed to load articles.');

                const result = await response.json() as PaginatedApiResponse<Article> | Article[];
                const loadedArticles = getResponseData(result);

                setArticles(loadedArticles);
                setRows([getDefaultArticleRow(loadedArticles)]);
            } catch (error) {
                console.error('Failed to load articles:', error);
                setFormMessage('Failed to load articles.');
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

    const addRow = () => setRows((current) => [...current, getDefaultArticleRow(articles)]);

    const removeRow = (rowIndex: number) => {
        setRows((current) => current.filter((_, index) => index !== rowIndex));
    };

    const updateRow = (rowIndex: number, key: keyof FormArticleRow, value: string) => {
        setRows((current) =>
            current.map((row, index) => (index === rowIndex ? { ...row, [key]: value } : row))
        );
    };

    const handleCreateOrderItems = async () => {
        if (rows.length === 0) {
            setFormMessage('At least one order row is required.');
            return;
        }

        const articlesPayload: ArticleCreateDto[] = rows.map((row) => ({
            id: Number(row.articleId),
            quantity: Number(row.quantity)
        }));

        const invalidArticleRow = articlesPayload.some((row) => Number.isNaN(row.id) || row.id <= 0);
        const invalidQuantityRow = articlesPayload.some((row) => Number.isNaN(row.quantity) || row.quantity <= 0 || !Number.isInteger(row.quantity));

        if (invalidArticleRow) {
            setFormMessage('Choose an article for each row.');
            return;
        }

        if (invalidQuantityRow) {
            setFormMessage('Quantity must be a whole number greater than 0 for every row.');
            return;
        }

        const payload: OrderRowCreateDto = {
            articles: articlesPayload
        };

        setIsSubmitting(true);
        setFormError(null);
        setFormSuccess(null);

        try {
            const response = await fetch(`/api/orderrows/create/${orderId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || 'Failed to create order rows.');
            }

            setFormSuccess('Order rows added successfully.');
            setRows([getDefaultArticleRow(articles)]);
            await onOrderItemCreated();
        } catch (error) {
            const message = error instanceof Error ? error.message : 'Failed to create order rows.';
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
                <Typography variant="h6">Add Order Items</Typography>

                {isVisible && (
                    <>
                        {formError && <Alert severity="error">{formError}</Alert>}
                        {formSuccess && <Alert severity="success">{formSuccess}</Alert>}

                        {rows.map((row, index) => (
                            <Stack key={`order-item-row-${index}`} direction={{ xs: 'column', md: 'row' }} spacing={2}>
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
                            onClick={() => void handleCreateOrderItems()}
                            disabled={isSubmitting || articles.length === 0}
                        >
                            {isSubmitting ? 'Adding...' : 'Add Order Items'}
                        </Button>
                    </>
                )}
            </Stack>
        </Paper>
    );
}

export default AddOrderItemCard;