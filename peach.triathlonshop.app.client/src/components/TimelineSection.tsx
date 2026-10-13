import { Box, Stack, Typography } from '@mui/material';
import type { Key, ReactNode } from 'react';

export interface TimelineSectionProps<TItem> {
    title: string;
    items: TItem[];
    emptyText: string;
    getKey: (item: TItem) => Key;
    getTitle: (item: TItem) => string;
    getTimestamp: (item: TItem) => string;
    renderDetails?: (item: TItem) => ReactNode;
    renderContent?: (item: TItem) => ReactNode;
}

export function TimelineSection<TItem>({
    title,
    items,
    emptyText,
    getKey,
    getTitle,
    getTimestamp,
    renderDetails,
    renderContent
}: TimelineSectionProps<TItem>) {
    return (
        <Box sx={{ p: 2 }}>
            <Stack spacing={2}>
                <Typography variant="h6">{title}</Typography>
                <Stack spacing={0}>
                    {items.map((item, index) => {
                        const isLast = index === items.length - 1;

                        return (
                            <Box key={getKey(item)} sx={{ display: 'flex', gap: 2, minHeight: 64 }}>
                                <Stack sx={{ alignItems: 'center' }}>
                                    <Box
                                        sx={{
                                            width: 10,
                                            height: 10,
                                            borderRadius: '50%',
                                            bgcolor: 'text.primary',
                                            mt: 0.75
                                        }}
                                    />
                                    {!isLast && (
                                        <Box sx={{ width: '1px', flex: 1, bgcolor: 'divider', my: 0.5 }} />
                                    )}
                                </Stack>
                                <Stack spacing={0.5} sx={{ pb: isLast ? 0 : 2 }}>
                                    {renderContent ? (
                                        renderContent(item)
                                    ) : (
                                        <>
                                            <Typography sx={{ fontWeight: 600 }}>{getTitle(item)}</Typography>
                                            <Typography variant="body2" color="text.secondary">
                                                {getTimestamp(item)}
                                            </Typography>
                                            {renderDetails?.(item)}
                                        </>
                                    )}
                                </Stack>
                            </Box>
                        );
                    })}
                    {items.length === 0 && (
                        <Typography variant="body2" color="text.secondary">
                            {emptyText}
                        </Typography>
                    )}
                </Stack>
            </Stack>
        </Box>
    );
}
