import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

export interface SignalRNegotiateResponse {
    url: string;
    accessToken: string;
}

export async function createSignalRConnection(negotiationEndpoint: string): Promise<HubConnection> {
    console.log(`Negotiating SignalR connection with endpoint: ${negotiationEndpoint}`);

    const negotiateResponse = await fetch(`/api/homepage/negotiation/${negotiationEndpoint}`);

    if (!negotiateResponse.ok) {
        console.log("Negotiation failed");

        throw new Error(`Failed to negotiate SignalR connection: ${negotiationEndpoint}`);
    }

    const { url, accessToken } = await negotiateResponse.json() as SignalRNegotiateResponse;
    console.log(`Negotiation successful. URL: ${url}`);

    return new HubConnectionBuilder()
        .withUrl(url, { accessTokenFactory: () => accessToken })
        .build();
}