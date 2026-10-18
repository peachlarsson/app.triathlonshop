import { Box, Stack, Typography } from '@mui/material';
import { usePaginatedData } from '../hooks/usePaginatedData';
import { DataTable, type ColumnDefinition } from '../components/DataTable';
import { PaginationControls } from '../components/PaginationControls';
import { TableFilters } from '../components/TableFilters';
import AddOrderCard from '../components/AddOrderCard';

interface Order {
    id: number;
    number: string;
    status: string;
    totalPrice: number;
    orderRowsCount: number;
    customerId: number;
    customerName: string;
    createdAt: string;
}

function App() {

    // Data fetching and pagination
    const [state, actions] = usePaginatedData<Order>({
        tablePageSize: 10,
        apiPageSize: 100,
        endpointPagination: '/api/orders',
        endpointSearchBar: '/api/orders',
        searchQueryParam: 'customerName'
    });

    // Pagination info and display data
    const paginationInfo = actions.getPaginationInfo();
    const displayData = actions.getDisplayData();

    // Column definitions
    const columns: ColumnDefinition[] = [
        { key: 'id', label: 'Id' },
        { key: 'number', label: 'Number' },
        { key: 'status', label: 'Status' },
        { key: 'totalPrice', label: 'Total Price', isPrice: true },
        { key: 'orderRowsCount', label: 'Order Rows Count' },
        { key: 'customerId', label: 'Customer Id' },
        { key: 'customerName', label: 'Customer Name' },
        { key: 'createdAt', label: 'Created Timestamp' },
    ];

    return (
        <Stack spacing={4}>
            <Box>
                <Typography id="ordersTableLabel" variant="h4" gutterBottom>
                    Orders
                </Typography>

                <TableFilters
                    searchQuery={state.searchQuery}
                    placeholder="Search by Customer Name"
                    onSearchChange={actions.setSearchQuery}
                    tableSize={state.tablePageSize}
                    onTableSizeChange={actions.setTablePageSize}
                    tableSizeOptions={[10, 50, 100, 500]}
                    totalCount={state.totalCount}
                />

                <PaginationControls
                    currentPage={paginationInfo.displayPage}
                    totalPages={paginationInfo.totalPages}
                    onPageChange={actions.goToPage}
                    isLoading={state.isLoading}
                />

                <DataTable
                    columns={columns}
                    data={displayData}
                    isLoading={state.isLoading}
                    tableId="ordersTableLabel"
                    sortBy={state.sortBy}
                    sortOrder={state.sortOrder}
                    onSortChange={(sortBy, sortOrder) => {
                        actions.setSortBy(sortBy);
                        actions.setSortOrder(sortOrder);
                    }}
                    isClickable={true}
                    rowIdKey="id"
                    navigateUrl="/orders"
                />

                <AddOrderCard onOrderCreated={actions.refresh} />
            </Box>
        </Stack>
    );
}

export default App;
