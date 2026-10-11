import { Box, Stack, Typography } from '@mui/material';
import { usePaginatedData } from '../hooks/usePaginatedData';
import { DataTable, type ColumnDefinition } from '../components/DataTable';
import { PaginationControls } from '../components/PaginationControls';
import { TableFilters } from '../components/TableFilters';

interface Article {
    id: number;
    number: string;
    name: string;
    unitPrice: number;
}

function App() {

    // Data fetching and pagination
    const [state, actions] = usePaginatedData<Article>({
        tablePageSize: 10,
        apiPageSize: 20,
        endpointPagination: '/api/articles',
        endpointSearchBar: '/api/articles',
        searchQueryParam: 'name'
    });

    // Pagination info and display data
    const paginationInfo = actions.getPaginationInfo();
    const displayData = actions.getDisplayData();

    // Column definitions
    const columns: ColumnDefinition[] = [
        { key: 'id', label: 'Id' },
        { key: 'number', label: 'Number' },
        { key: 'name', label: 'Name' },
        { key: 'unitPrice', label: 'Unit Price', isPrice: true },
    ];

    return (
        <Stack spacing={4}>
            <Box>
                <Typography id="articlesTableLabel" variant="h4" gutterBottom>
                    Articles
                </Typography>

                <TableFilters
                    searchQuery={state.searchQuery}
                    placeholder="Search by Article Name"
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
                    tableId="articlesTableLabel"
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
