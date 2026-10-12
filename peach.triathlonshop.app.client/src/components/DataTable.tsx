import {
    CircularProgress,
    Paper,
    Stack,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    TableSortLabel,
    Typography
} from '@mui/material';

export interface ColumnDefinition
{
    key: string;
    label: string;
    isPrice?: boolean;
    sortable?: boolean;
    sortKey?: string;
}

export interface DataTableProps
{
    columns: ColumnDefinition[];
    data: object[];
    isLoading?: boolean;
    tableId?: string;
    sortBy?: string;
    sortOrder?: string;
    onSortChange?: (sortBy: string, sortOrder: string) => void;
}

function LoadingState()
{
    return (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', p: 2 }}>
            <CircularProgress size={18} />
            <Typography variant="body2">Loading data...</Typography>
        </Stack>
    );
}

function getCellValue(column: ColumnDefinition, row: object): React.ReactNode
{
    const value = (row as Record<string, unknown>)[column.key];

    if (column.isPrice && typeof value === 'number') {
        return value.toFixed(2);
    }

    return value as React.ReactNode;
}

function toSortKey(columnKey: string)
{
    return columnKey.charAt(0).toUpperCase() + columnKey.slice(1);
}

function getRowKey(row: object, rowIndex: number): React.Key
{
    const record = row as Record<string, unknown>;
    const id = record.id ?? record.Id;

    if (typeof id === 'string' || typeof id === 'number') {
        return id;
    }

    return rowIndex;
}

export function DataTable(
{
    columns,
    data,
    isLoading = false,
    tableId = 'dataTable',
    sortBy,
    sortOrder,
    onSortChange
}: DataTableProps) {

    const handleSortClick = (column: ColumnDefinition) =>
    {
        if (!onSortChange) return;

        const nextSortBy = column.sortKey ?? toSortKey(column.key);
        const isSameColumn = sortBy === nextSortBy;
        const nextSortOrder = isSameColumn && sortOrder?.toLowerCase() === 'asc' ? 'desc' : 'asc';
 
        onSortChange(nextSortBy, nextSortOrder);
    };

    return (
        <TableContainer component={Paper} variant="outlined" sx={{ mb: 3 }}>
            <Table size="small" aria-labelledby={tableId}>
                <TableHead>
                    <TableRow>
                        {columns.map((column) => {
                            const sortKey = column.sortKey ?? toSortKey(column.key);
                            const isActive = sortBy === sortKey;
                            const direction = sortOrder?.toLowerCase() === 'desc' ? 'desc' : 'asc';

                            return (
                                <TableCell
                                    key={String(column.key)}
                                    sortDirection={isActive ? direction : false}
                                    sx={{ cursor: onSortChange ? 'pointer' : 'default', userSelect: 'none' }}
                                    onClick={onSortChange ? () => handleSortClick(column) : undefined}
                                >
                                    <TableSortLabel active={isActive} direction={direction} hideSortIcon={!onSortChange}>
                                        {column.label}
                                    </TableSortLabel>
                                </TableCell>
                            );
                        })}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {isLoading ? (
                        <TableRow>
                            <TableCell colSpan={columns.length}>
                                <LoadingState />
                            </TableCell>
                        </TableRow>
                    ) : data.length > 0 ? (
                        data.map((row, rowIndex) => (
                            <TableRow key={getRowKey(row, rowIndex)} hover>
                                {columns.map((column) => (
                                    <TableCell key={String(column.key)}>
                                        {getCellValue(column, row)}
                                    </TableCell>
                                ))}
                            </TableRow>
                        ))
                    ) : null}
                </TableBody>
            </Table>
        </TableContainer>
    );
}