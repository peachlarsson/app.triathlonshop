import { FormControl, InputLabel, MenuItem, Select, Stack, TextField, Typography } from '@mui/material';

export interface SortOption {
    value: string;
    label: string;
}

export interface TableFiltersProps {
    searchQuery: string;
    placeholder: string;
    onSearchChange: (query: string) => void;
    tableSize: number;
    onTableSizeChange: (size: number) => void;
    tableSizeOptions?: number[];
    totalCount: number;
}

export function TableFilters({
    searchQuery,
    placeholder,
    onSearchChange,
    tableSize,
    onTableSizeChange,
    tableSizeOptions = [10, 50, 100, 500],
    totalCount
}: TableFiltersProps) {
    return (
        <>
            <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
                <TextField
                    placeholder={placeholder}
                    variant="outlined"
                    size="small"
                    value={searchQuery}
                    onChange={(e) => onSearchChange(e.target.value)}
                    sx={{ flex: 1 }}
                />
                <FormControl variant="outlined" size="small" sx={{ minWidth: 120 }}>
                    <InputLabel>Rows</InputLabel>
                    <Select
                        value={tableSize}
                        onChange={(e) => onTableSizeChange(Number(e.target.value))}
                        label="Rows"
                    >
                        {tableSizeOptions.map((sizeOption) => (
                            <MenuItem key={sizeOption} value={sizeOption}>
                                {sizeOption}
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </Stack>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Items: {totalCount}
            </Typography>
        </>
    );
}
