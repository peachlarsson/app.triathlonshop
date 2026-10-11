import { Box, Stack, Typography } from '@mui/material';
import { usePaginatedData } from '../hooks/usePaginatedData';
import { DataTable, type ColumnDefinition } from '../components/DataTable';
import { PaginationControls } from '../components/PaginationControls';
import { TableFilters } from '../components/TableFilters';

interface Customer {
    id: number;
    email: string;
    name: string;
}

function App() {

    // Data fetching and pagination
    const [state, actions] = usePaginatedData<Customer>({
        tablePageSize: 10,
        apiPageSize: 100,
        endpointPagination: '/api/customers',
        endpointSearchBar: '/api/customers',
        searchQueryParam: 'name'
    });

    // Pagination info and display data
    const paginationInfo = actions.getPaginationInfo();
    const displayData = actions.getDisplayData();

    // Column definitions
    const columns: ColumnDefinition[] = [
        { key: 'id', label: 'Id' },
        { key: 'email', label: 'Email' },
        { key: 'name', label: 'Name' },
    ];

    return (
        <Stack spacing={4}>
            <Box>
                <Typography id="customersTableLabel" variant="h4" gutterBottom>
                    Customers
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
                    tableId="customersTableLabel"
                    sortBy={state.sortBy}
                    sortOrder={state.sortOrder}
                    onSortChange={(sortBy, sortOrder) => {
                        actions.setSortBy(sortBy);
                        actions.setSortOrder(sortOrder);
                    }}
                />
            </Box>
        </Stack>
    );
}

export default App;
