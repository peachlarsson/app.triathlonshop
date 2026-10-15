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
import { useNavigate } from 'react-router-dom';

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
    isClickable?: boolean;
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

function getRowId(row: object): string | number | undefined
{
    const record = row as Record<string, unknown>;
    const id = record.id ?? record.Id;

    if (typeof id === 'string' || typeof id === 'number') {
        return id;
    }

    return undefined;
}

function getRowKey(row: object, rowIndex: number): React.Key {
    const rowId = getRowId(row);

    if (rowId !== undefined) {
        return `${String(rowId)}-${rowIndex}`;
    }

    return rowIndex;
}

export function DataTable(
    {
        columns,
        data,
        isClickable = false,
        isLoading = false,
        tableId = 'dataTable',
        sortBy,
        sortOrder,
        onSortChange
    }: DataTableProps) {

    const navigate = useNavigate();

    const handleSortClick = (column: ColumnDefinition) =>
    {
        if (!onSortChange) return;

        const nextSortBy = column.sortKey ?? toSortKey(column.key);
        const isSameColumn = sortBy === nextSortBy;
        const nextSortOrder = isSameColumn && sortOrder?.toLowerCase() === 'asc' ? 'desc' : 'asc';

        onSortChange(nextSortBy, nextSortOrder);
    };

    return (
        <TableContainer component={Paper} variant="outlined" sx={{ mb: 3, width: 'fit-content' }}>
            <Table size="small" aria-labelledby={tableId} sx={{ tableLayout: 'auto', width: 'max-content' }}>
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
                                    sx={{ cursor: onSortChange ? 'pointer' : 'default', userSelect: 'none', whiteSpace: 'nowrap' }}
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
                            <TableCell colSpan={columns.length} sx={{ whiteSpace: 'nowrap' }}>
                                <LoadingState />
                            </TableCell>
                        </TableRow>
                    ) : data.length > 0 ?
                        (
                            data.map((row, rowIndex) => 
                            {
                                const rowId = getRowId(row);
                                const canClick = isClickable && rowId !== undefined;

                                return (
                                    <TableRow
                                        key={getRowKey(row, rowIndex)}
                                        hover={canClick}
                                        onClick={canClick ? () => navigate(`/orders/${rowId}`) : undefined}
                                        sx={canClick ? { cursor: 'pointer' } : undefined}
                                    >
                                        {columns.map((column) => (
                                            <TableCell key={String(column.key)} sx={{ whiteSpace: 'nowrap' }}>
                                                {getCellValue(column, row)}
                                            </TableCell>
                                        ))}
                                    </TableRow>
                                );
                            })
                        )
                        : null
                    }
                </TableBody>
            </Table>
        </TableContainer>
    );
}