import { Button, Stack, Typography } from '@mui/material';

export interface PaginationControlsProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
    isLoading?: boolean;
}

function getVisiblePages(currentPage: number, totalPages: number, maxButtons = 3): number[] {
    if (totalPages <= 0) return [];
    if (totalPages <= maxButtons) {
        return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    const half = Math.floor(maxButtons / 2);
    let start = Math.max(1, currentPage - half);
    let end = start + maxButtons - 1;

    if (end > totalPages) {
        end = totalPages;
        start = end - maxButtons + 1;
    }

    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
}

export function PaginationControls({
    currentPage,
    totalPages,
    onPageChange,
    isLoading = false
}: PaginationControlsProps) {
    const displayTotalPages = Math.max(totalPages, 1);
    const displayCurrentPage = Math.min(Math.max(currentPage, 1), displayTotalPages);
    const pages = getVisiblePages(displayCurrentPage, totalPages, 3);

    const firstVisible = pages[0];
    const lastVisible = pages[pages.length - 1];
    const canGoPrev = displayCurrentPage > 1 && !isLoading && totalPages > 0;
    const canGoNext = displayCurrentPage < displayTotalPages && !isLoading && totalPages > 0;

    return (
        <Stack direction="row" spacing={1} sx={{ mt: 2, mb: 2, justifyContent: 'center', alignItems: 'center' }}>
            <Button
                variant="outlined"
                size="small"
                disabled={!canGoPrev}
                onClick={() => onPageChange(displayCurrentPage - 1)}
            >
                {'<'}
            </Button>

            {firstVisible > 1 && (
                <>
                    <Button
                        variant={displayCurrentPage === 1 ? 'contained' : 'outlined'}
                        size="small"
                        disabled={isLoading}
                        onClick={() => onPageChange(1)}
                    >
                        1
                    </Button>
                    {firstVisible > 2 && <Typography variant="body2">…</Typography>}
                </>
            )}

            {pages.map((page) => (
                <Button
                    key={page}
                    variant={page === displayCurrentPage ? 'contained' : 'outlined'}
                    size="small"
                    disabled={isLoading}
                    onClick={() => onPageChange(page)}
                >
                    {page}
                </Button>
            ))}

            {lastVisible < totalPages && (
                <>
                    {lastVisible < totalPages - 1 && <Typography variant="body2">…</Typography>}
                    <Button
                        variant={displayCurrentPage === totalPages ? 'contained' : 'outlined'}
                        size="small"
                        disabled={isLoading}
                        onClick={() => onPageChange(totalPages)}
                    >
                        {totalPages}
                    </Button>
                </>
            )}

            <Button
                variant="outlined"
                size="small"
                disabled={!canGoNext}
                onClick={() => onPageChange(displayCurrentPage + 1)}
            >
                {'>'}
            </Button>
        </Stack>
    );
}