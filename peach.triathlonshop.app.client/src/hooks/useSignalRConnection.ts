import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

export interface SignalRNegotiateResponse {
    url: string;
    accessToken: string;
}

export async function createSignalRConnection(controllerEndpoint: string): Promise<HubConnection> {

    const negotiateResponse = await fetch(`${controllerEndpoint}`);

    if (!negotiateResponse.ok) {
        throw new Error("Negotiation failed.");
    }

    const { url, accessToken } = await negotiateResponse.json() as SignalRNegotiateResponse;

    console.log("Negotiation successful.");

    return new HubConnectionBuilder()
        .withUrl(url, { accessTokenFactory: () => accessToken })
        .build();
}