import { Typography } from '@mui/material';
import type { ReactNode } from 'react';

export type CombinedTimelineEntry =
    | {
        id: string;
        type: 'notification';
        typeLabel: 'Notification';
        title: string;
        timestamp: string;
        details: ReactNode;
    }
    | {
        id: string;
        type: 'transport';
        typeLabel: 'Transport';
        title: string;
        timestamp: string;
        details: ReactNode;
    }
    | {
        id: string;
        type: 'invoice';
        typeLabel: 'Invoice';
        title: string;
        timestamp: string;
        details?: ReactNode;
    };

export function CombinedTimelineItem({ item }: { item: CombinedTimelineEntry }) {
    return (
        <>
            <Typography sx={{ fontWeight: 600 }}>{`${item.typeLabel}: ${item.title}`}</Typography>
            <Typography variant="body2" color="text.secondary">
                {item.timestamp}
            </Typography>
            {item.details}
        </>
    );
}
