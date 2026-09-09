import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { MessageService } from 'primeng/api';
import { Order } from 'src/app/demo/api/models';
import { OrderController } from 'src/app/services/fastfood.service';
import { OrderHubService } from 'src/app/services/order-hub.service';
import { eOrderState } from 'src/app/enums/eOrderState';
import { eRole } from 'src/app/enums/eRole';
import { AuthService } from '../../auth/auth.service';

// Customer-facing "monitor" page: shows only Created/Ready orders, grouped by status, live via SignalR.
@Component({
    selector: 'app-order-status',
    templateUrl: './order-status.component.html',
    styleUrls: ['./order-status.component.scss'],
    providers: [MessageService]
})
export class OrderStatusComponent implements OnInit, OnDestroy {
    activeOrders: Order[] = [];
    private hubSubscriptions: Subscription[] = [];

    // Pregled role is read-only monitoring - no Cancel/Completed action buttons for it.
    canManageOrders: boolean = false;

    eOrderState = eOrderState;

    constructor(
        private orderController: OrderController,
        private messageService: MessageService,
        private orderHubService: OrderHubService,
        private authService: AuthService) { }

    ngOnInit() {
        this.canManageOrders = !this.authService.hasRole(eRole.Pregled);

        this.loadActiveOrders();

        this.orderHubService.connect();
        this.hubSubscriptions.push(
            this.orderHubService.orderCreated$.subscribe(order => this.onOrderCreated(order)),
            this.orderHubService.orderStateChanged$.subscribe(order => this.onOrderStateChanged(order))
        );
    }

    ngOnDestroy() {
        this.hubSubscriptions.forEach(subscription => subscription.unsubscribe());
    }

    loadActiveOrders() {
        this.orderController.GetAllOrders().subscribe({
            next: result => {
                this.activeOrders = (result ?? []).filter(o => this.isDisplayedState(o.state));
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING ORDERS LOADING', life: 3000 });
            }
        });
    }

    get preparingOrders(): Order[] {
        return this.activeOrders.filter(o => this.stateEquals(o.state, eOrderState.Created));
    }

    get readyOrders(): Order[] {
        return this.activeOrders.filter(o => this.stateEquals(o.state, eOrderState.Ready));
    }

    private isDisplayedState(state: eOrderState): boolean {
        return this.stateEquals(state, eOrderState.Created) || this.stateEquals(state, eOrderState.Ready);
    }

    // Backend serializes Order.State as its enum name (e.g. "Created"), not the numeric value, so compare both forms.
    private stateEquals(state: eOrderState, target: eOrderState): boolean {
        return state === target || (state as any) === eOrderState[target];
    }

    onOrderCreated(order: Order) {
        if (!order.id || !this.isDisplayedState(order.state) || this.activeOrders.some(o => o.id === order.id)) {
            return;
        }

        this.activeOrders = [...this.activeOrders, order];
    }

    onOrderStateChanged(order: Order) {
        if (!order.id) {
            return;
        }

        if (this.isDisplayedState(order.state)) {
            const index = this.activeOrders.findIndex(o => o.id === order.id);
            this.activeOrders = index === -1
                ? [...this.activeOrders, order]
                : this.activeOrders.map(o => o.id === order.id ? order : o);
        } else {
            this.activeOrders = this.activeOrders.filter(o => o.id !== order.id);
        }
    }

    completeOrder(order: Order) {
        this.updateOrderState(order, eOrderState.Completed, 'Order Completed');
    }

    cancelOrder(order: Order) {
        this.updateOrderState(order, eOrderState.Cancelled, 'Order Cancelled');
    }

    private updateOrderState(order: Order, state: eOrderState, successDetail: string) {
        const updatedOrder: Order = { ...order, state };

        this.orderController.UpdateOrder(updatedOrder).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Successful', detail: successDetail, life: 3000 });
                // SignalR's orderStateChanged$ also updates the list, but update immediately so the UI doesn't wait on the round trip.
                this.onOrderStateChanged(updatedOrder);
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING ORDER UPDATE', life: 3000 });
            }
        });
    }
}
