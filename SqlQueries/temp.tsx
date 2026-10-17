import { useCallback, useEffect, useRef, useState } from 'react';

export interface PaginatedResponse<T> {
    data: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}

export interface UsePaginatedDataOptions {
    tablePageSize: number;
    apiPageSize: number;
    endpointPagination: string;
    endpointSearchBar: string;
    searchQueryParam?: string;
}

export interface UsePaginatedDataState<T> {
    data: T[] | undefined;
    isLoading: boolean;
    totalCount: number;
    currentDisplayPage: number;
    searchQuery: string;
    sortBy: string;
    sortOrder: string;
    tablePageSize: number;
}

export interface UsePaginatedDataActions<T> {
    setSearchQuery: (query: string) => void;
    setSortBy: (sortBy: string) => void;
    setSortOrder: (sortOrder: string) => void;
    setTablePageSize: (size: number) => void;
    goToPage: (page: number) => void;
    getPaginationInfo: () => { displayPage: number; totalPages: number };
    getDisplayData: () => T[];
}

export function usePaginatedData<T>(
    options: UsePaginatedDataOptions
): [UsePaginatedDataState<T>, UsePaginatedDataActions<T>] {
    const {
        tablePageSize: initialTablePageSize,
        apiPageSize,
        endpointPagination,
        endpointSearchBar,
        searchQueryParam,
    } = options;

    const [data, setData] = useState<T[] | undefined>(undefined);
    const [isLoading, setIsLoading] = useState(false);
    const [totalCount, setTotalCount] = useState(0);
    const [currentApiPage, setCurrentApiPage] = useState(1);
    const [currentDisplayPage, setCurrentDisplayPage] = useState(1);
    const [searchQuery, setSearchQuery] = useState('');
    const [sortBy, setSortBy] = useState('Id');
    const [sortOrder, setSortOrder] = useState('Asc');
    const [tablePageSize, setTablePageSize] = useState(initialTablePageSize);

    const batchCacheRef = useRef<Map<string, T[]>>(new Map());
    const prevFiltersRef = useRef({ search: '', sortBy: 'Id', sortOrder: 'Asc', tablePageSize: initialTablePageSize });

    const getCacheKey = useCallback(
        (pageNumber: number, searchValue: string, sortByValue: string, sortOrderValue: string, pageSize: number) => {
            const normalizedSearch = searchValue.trim().toLowerCase();
            return `${pageNumber}|${pageSize}|${sortByValue}|${sortOrderValue}|${normalizedSearch}`;
        },
        []
    );

    const fetchPaginatedData = useCallback(
        async (pageNumber: number, sortByValue: string, sortOrderValue: string, pageSize: number) => {
            const params = new URLSearchParams({
                pageNumber: pageNumber.toString(),
                pageSize: pageSize.toString(),
                sortBy: sortByValue,
                sortOrder: sortOrderValue
            });

            try {
                const response = await fetch(`${endpointPagination}?${params}`);
                if (response.ok) {
                    const result: PaginatedResponse<T> = await response.json();
                    const cacheKey = getCacheKey(pageNumber, '', sortByValue, sortOrderValue, pageSize);
                    setData(result.data);
                    batchCacheRef.current.set(cacheKey, result.data);
                    setTotalCount(result.totalCount);
                } else {
                    console.error('Failed to fetch data:', response.status, response.statusText);
                }
            } catch (error) {
                console.error('Network error while fetching paginated data:', error);
            }
        },
        [endpointPagination, getCacheKey]
    );

    const sortClientResults = useCallback((result: T[], sortByValue: string, sortOrderValue: string) => {
        return [...result].sort((a, b) => {
            const direction = sortOrderValue.toLowerCase() === 'desc' ? -1 : 1;
            const rowA = a as Record<string, unknown>;
            const rowB = b as Record<string, unknown>;
            const directKey = sortByValue;
            const fallbackKey = sortByValue.charAt(0).toLowerCase() + sortByValue.slice(1);

            const valueA = rowA[directKey] ?? rowA[fallbackKey];
            const valueB = rowB[directKey] ?? rowB[fallbackKey];

            if (typeof valueA === 'number' && typeof valueB === 'number') {
                return (valueA - valueB) * direction;
            }

            return String(valueA ?? '').localeCompare(String(valueB ?? '')) * direction;
        });
    }, []);

    const fetchSearchData = useCallback(
        async (searchQueryValue: string, sortByValue: string, sortOrderValue: string, pageNumber: number, pageSize: number) =>
        {
            const searchParams = new URLSearchParams({
                pageNumber: pageNumber.toString(),
                pageSize: pageSize.toString(),
                sortBy: sortByValue,
                sortOrder: sortOrderValue,
                [searchQueryParam]: searchQueryValue
            });

            try {
                const searchResponse = await fetch(`${endpointSearchBar}?${searchParams}`);

                if (searchResponse.ok) {
                    const result: PaginatedResponse<T> | T[] = await searchResponse.json();

                    if (Array.isArray(result)) {
                        const sortedResult = sortClientResults(result, sortByValue, sortOrderValue);
                        const cacheKey = getCacheKey(1, searchQueryValue, sortByValue, sortOrderValue, pageSize);
                        setData(sortedResult);
                        batchCacheRef.current.set(cacheKey, sortedResult);
                        setTotalCount(sortedResult.length);
                        return;
                    }

                    const cacheKey = getCacheKey(pageNumber, searchQueryValue, sortByValue, sortOrderValue, pageSize);
                    setData(result.data);
                    batchCacheRef.current.set(cacheKey, result.data);
                    setTotalCount(result.totalCount);
                } else {
                    console.error('Failed to fetch data:', searchResponse.status, searchResponse.statusText);
                }
            } catch (error) {
                console.error('Network error while fetching search data:', error);
            }
        },
        [endpointSearchBar, searchQueryParam, sortClientResults, getCacheKey]
    );

    const fetchData = useCallback(
        async (pageNumber: number, search: string, sortByValue: string, sortOrderValue: string) =>
        {
            setIsLoading(true);
            const normalizedSearchQuery = search.trim();
            const pageSize = Math.max(apiPageSize, tablePageSize);

            try {

                if (normalizedSearchQuery.length > 0)
                {
                    await fetchSearchData(normalizedSearchQuery, sortByValue, sortOrderValue, pageNumber, pageSize);
                    return;
                }

                await fetchPaginatedData(pageNumber, sortByValue, sortOrderValue, pageSize);
            } finally {
                setIsLoading(false);
            }
        },
        [apiPageSize, tablePageSize, fetchSearchData, fetchPaginatedData]
    );

    // Handle filter changes
    useEffect(() => {
        const filtersChanged =
            prevFiltersRef.current.search !== searchQuery ||
            prevFiltersRef.current.sortBy !== sortBy ||
            prevFiltersRef.current.sortOrder !== sortOrder ||
            prevFiltersRef.current.tablePageSize !== tablePageSize;

        const timer = setTimeout(() => {
            prevFiltersRef.current = { search: searchQuery, sortBy, sortOrder, tablePageSize };
            if (filtersChanged) {
                setCurrentDisplayPage(1);
                setCurrentApiPage(1);
                batchCacheRef.current.clear();
                fetchData(1, searchQuery, sortBy, sortOrder);
            }
        }, 300);

        return () => clearTimeout(timer);
    }, [searchQuery, sortBy, sortOrder, tablePageSize, fetchData]);

    // Handle pagination and caching
    useEffect(() => {
        const pageSize = Math.max(apiPageSize, tablePageSize);
        const cacheKey = getCacheKey(currentApiPage, searchQuery, sortBy, sortOrder, pageSize);
        const cached = batchCacheRef.current.get(cacheKey);

        if (cached) {
            setData(cached);
        } else {
            fetchData(currentApiPage, searchQuery, sortBy, sortOrder);
        }
    }, [currentApiPage, searchQuery, sortBy, sortOrder, fetchData, apiPageSize, tablePageSize, getCacheKey]);

    // Handle table page size changes
    const getPaginationInfo = useCallback(() => {
        const totalPages = Math.ceil(totalCount / tablePageSize);
        const effectiveApiPageSize = Math.max(apiPageSize, tablePageSize);
        const itemsBeforeCurrentBatch = (currentApiPage - 1) * effectiveApiPageSize;
        const currentBatchStartPage = Math.floor(itemsBeforeCurrentBatch / tablePageSize) + 1;
        const displayPage = data ? currentBatchStartPage + currentDisplayPage - 1 : 1;

        return { displayPage, totalPages };
    }, [totalCount, tablePageSize, currentApiPage, apiPageSize, currentDisplayPage, data]);

    // Get the data to display on the current table page
    const getDisplayData = useCallback(() => {
        return data?.slice(
            (currentDisplayPage - 1) * tablePageSize,
            currentDisplayPage * tablePageSize
        ) ?? [];
    }, [data, currentDisplayPage, tablePageSize]);

    // Navigate to a specific display page
    const goToPage = useCallback(
        (newPage: number) => {
            const totalPages = Math.ceil(totalCount / tablePageSize);
            if (newPage < 1 || newPage > totalPages) return;

            const effectiveApiPageSize = Math.max(apiPageSize, tablePageSize);
            const totalApiPages = Math.ceil(totalCount / effectiveApiPageSize);
            const itemsBeforeNewPage = (newPage - 1) * tablePageSize;
            let neededApiPage = Math.floor(itemsBeforeNewPage / effectiveApiPageSize) + 1;
            let localPageInBatch = Math.ceil(((itemsBeforeNewPage % effectiveApiPageSize) / tablePageSize) + 1);

            if (
                data &&
                neededApiPage === currentApiPage &&
                (localPageInBatch - 1) * tablePageSize >= data.length &&
                currentApiPage < totalApiPages
            ) {
                neededApiPage = currentApiPage + 1;
                localPageInBatch = 1;
            }

            if (neededApiPage !== currentApiPage && neededApiPage <= totalApiPages) {
                setCurrentApiPage(neededApiPage);
                setCurrentDisplayPage(localPageInBatch);
            } else if (neededApiPage === currentApiPage) {
                setCurrentDisplayPage(localPageInBatch);
            }
        },
        [totalCount, tablePageSize, apiPageSize, currentApiPage, data]
    );

    const state: UsePaginatedDataState<T> = {
        data,
        isLoading,
        totalCount,
        currentDisplayPage,
        searchQuery,
        sortBy,
        sortOrder,
        tablePageSize
    };

    const actions: UsePaginatedDataActions<T> = {
        setSearchQuery,
        setSortBy,
        setSortOrder,
        setTablePageSize,
        goToPage,
        getPaginationInfo,
        getDisplayData
    };

    return [state, actions];
}
