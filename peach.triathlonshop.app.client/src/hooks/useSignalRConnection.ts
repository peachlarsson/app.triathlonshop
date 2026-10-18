import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

export interface SignalRNegotiateResponse {
    url: string;
    accessToken: string;
}

export async function createSignalRConnection(controllerEndpoint: string): Promise<HubConnection> {

    const negotiateResponse = await fetch(`${controllerEndpoint}`);

    if (!negotiateResponse.ok) {
        throw new Error(`Failed to negotiate SignalR connection with ${controllerEndpoint}. Error: ${negotiateResponse.status} ${negotiateResponse.statusText}`);
    }

    const { url, accessToken } = await negotiateResponse.json() as SignalRNegotiateResponse;

    return new HubConnectionBuilder()
        .withUrl(url, { accessTokenFactory: () => accessToken })
        .build();
}