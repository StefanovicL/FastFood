import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { Order } from 'src/app/demo/api/models';
import { environment } from 'src/environments/environment';

// Wraps the OrderHub SignalR connection so order pages can react to server-pushed changes.
@Injectable({ providedIn: 'root' })
export class OrderHubService {
    private hubConnection: signalR.HubConnection;

    orderCreated$ = new Subject<Order>();
    orderStateChanged$ = new Subject<Order>();

    connect(): void {
        if (this.hubConnection) {
            return;
        }

        const hubUrl = `${environment.apiUrl.replace(/\/api\/?$/, '')}/hubs/order`;

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        this.hubConnection.on('OrderCreated', (order: Order) => this.orderCreated$.next(order));
        this.hubConnection.on('OrderStateChanged', (order: Order) => this.orderStateChanged$.next(order));

        this.hubConnection.start().catch(error => console.error('OrderHub connection error:', error));
    }
}
