import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { MessageService } from 'primeng/api';
import { Order } from 'src/app/demo/api/models';
import { OrderController } from 'src/app/services/fastfood.service';
import { OrderHubService } from 'src/app/services/order-hub.service';
import { eOrderState } from 'src/app/enums/eOrderState';

@Component({
    selector: 'app-active-orders',
    templateUrl: './active-orders.component.html',
    styleUrls: ['./active-orders.component.scss'],
    providers: [MessageService]
})
export class ActiveOrdersComponent implements OnInit, OnDestroy {
    orders: Order[] = [];
    expandedOrderId: number | null = null;
    elapsedTimes: { [orderId: number]: string } = {};
    private timerHandle: any;
    private hubSubscriptions: Subscription[] = [];

    constructor(private orderController: OrderController, private messageService: MessageService, private orderHubService: OrderHubService) { }

    ngOnInit() {
        this.loadActiveOrders();

        this.timerHandle = setInterval(() => this.updateElapsedTimes(), 1000);

        this.orderHubService.connect();
        this.hubSubscriptions.push(
            this.orderHubService.orderCreated$.subscribe(order => this.onOrderCreated(order)),
            this.orderHubService.orderStateChanged$.subscribe(order => this.onOrderStateChanged(order))
        );
    }

    ngOnDestroy() {
        if (this.timerHandle) {
            clearInterval(this.timerHandle);
        }

        this.hubSubscriptions.forEach(subscription => subscription.unsubscribe());
    }

    onOrderCreated(order: Order) {
        if (order.state !== eOrderState.Created || !order.id || this.orders.some(o => o.id === order.id)) {
            return;
        }

        this.orders = this.sortOrdersByTimer([...this.orders, order]);
        this.updateElapsedTimes();
    }

    onOrderStateChanged(order: Order) {
        if (!order.id) {
            return;
        }

        if (order.state === eOrderState.Created) {
            const index = this.orders.findIndex(o => o.id === order.id);
            if (index === -1) {
                this.orders = this.sortOrdersByTimer([...this.orders, order]);
            } else {
                this.orders = this.sortOrdersByTimer(this.orders.map(o => o.id === order.id ? order : o));
            }
            this.updateElapsedTimes();
        } else {
            this.orders = this.orders.filter(o => o.id !== order.id);
        }
    }

    loadActiveOrders() {
        this.orderController.GetActiveOrders().subscribe({
            next: result => {
                this.orders = this.sortOrdersByTimer(result);
                this.updateElapsedTimes();
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING ORDERS LOADING', life: 3000 });
            }
        });
    }

    updateElapsedTimes() {
        this.orders.forEach(order => {
            if (!order.id || !order.orderReceivedDateTime)
                return;

            this.elapsedTimes[order.id] = this.formatElapsed(order.orderReceivedDateTime);
        });
    }

    private sortOrdersByTimer(orders: Order[]): Order[] {
        return orders.sort((first, second) => {
            const firstReceived = new Date(first.orderReceivedDateTime ?? 0).getTime();
            const secondReceived = new Date(second.orderReceivedDateTime ?? 0).getTime();

            return firstReceived - secondReceived;
        });
    }

    formatElapsed(receivedDateTime: string): string {
        const diffMs = Date.now() - new Date(receivedDateTime).getTime();
        const totalSeconds = Math.max(0, Math.floor(diffMs / 1000));

        const hours = Math.floor(totalSeconds / 3600);
        const minutes = Math.floor((totalSeconds % 3600) / 60);
        const seconds = totalSeconds % 60;

        const pad = (n: number) => n.toString().padStart(2, '0');

        return hours > 0
            ? `${pad(hours)}:${pad(minutes)}:${pad(seconds)}`
            : `${pad(minutes)}:${pad(seconds)}`;
    }

    getUnitLabel(unit: any): string {
        switch (unit) {
            case 1:
            case 'g':
                return 'g';
            case 2:
            case 'ml':
                return 'ml';
            case 3:
            case 'kom':
                return 'pcs';
            default:
                return '';
        }
    }

    isExpanded(order: Order): boolean {
        return order.id === this.expandedOrderId;
    }

    toggleDetails(order: Order) {
        if (!order.id) {
            return;
        }

        this.expandedOrderId = this.isExpanded(order) ? null : order.id;
    }

    markAsReady(order: Order) {
        const updatedOrder: Order = { ...order, state: eOrderState.Ready };

        this.orderController.UpdateOrder(updatedOrder).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Order Ready', life: 3000 });
                this.orders = this.orders.filter(o => o.id !== order.id);
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING ORDER UPDATE', life: 3000 });
            }
        });
    }
}
