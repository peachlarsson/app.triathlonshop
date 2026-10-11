import { Box, Stack, Typography } from '@mui/material';
import { usePaginatedData } from '../hooks/usePaginatedData';
import { DataTable, type ColumnDefinition } from '../components/DataTable';
import { PaginationControls } from '../components/PaginationControls';
import { TableFilters } from '../components/TableFilters';

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
}

function App() {

    // Data fetching and pagination
    const [orderRowState, orderRowActions] = usePaginatedData<OrderRow>({
        tablePageSize: 10,
        apiPageSize: 20,
        endpointPagination: '/api/orderrows',
        endpointSearchBar: '/api/orderrows',
        searchQueryParam: 'articleName'
    });

    // Pagination info and display data
    const orderRowPaginationInfo = orderRowActions.getPaginationInfo();
    const orderRowDisplayData = orderRowActions.getDisplayData();

    // Column definitions
    const orderRowColumns: ColumnDefinition[] = [
        { key: 'id', label: 'Id' },
        { key: 'orderId', label: 'Order Id' },
        { key: 'orderNumber', label: 'Order Number' },
        { key: 'lineNumber', label: 'Line Number' },
        { key: 'articleName', label: 'Article Name' },
        { key: 'unitPrice', label: 'Unit Price', isPrice: true },
        { key: 'quantity', label: 'Quantity' },
        { key: 'rowPrice', label: 'Row Price', isPrice: true },
        { key: 'customerId', label: 'Customer Id' },
        { key: 'customerName', label: 'Customer Name' }
    ];

    return (
        <Stack spacing={4}>
            <Box>
                <Typography id="orderRowsTableLabel" variant="h4" gutterBottom>
                    Order Rows
                </Typography>

                <TableFilters
                    searchQuery={orderRowState.searchQuery}
                    placeholder="Search by Article Name"
                    onSearchChange={orderRowActions.setSearchQuery}
                    tableSize={orderRowState.tablePageSize}
                    onTableSizeChange={orderRowActions.setTablePageSize}
                    tableSizeOptions={[10, 50, 100, 500]}
                    totalCount={orderRowState.totalCount}
                />

                <PaginationControls
                    currentPage={orderRowPaginationInfo.displayPage}
                    totalPages={orderRowPaginationInfo.totalPages}
                    onPageChange={orderRowActions.goToPage}
                    isLoading={orderRowState.isLoading}
                />

                <DataTable
                    columns={orderRowColumns}
                    data={orderRowDisplayData}
                    isLoading={orderRowState.isLoading}
                    tableId="orderRowsTableLabel"
                    sortBy={orderRowState.sortBy}
                    sortOrder={orderRowState.sortOrder}
                    onSortChange={(sortBy, sortOrder) => {
                        orderRowActions.setSortBy(sortBy);
                        orderRowActions.setSortOrder(sortOrder);
                    }}
                />
            </Box>
        </Stack>
    );
}

export default App;
