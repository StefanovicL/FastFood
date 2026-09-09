import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { MessageService, SelectItem } from 'primeng/api';
import { CartItem, Order, Product } from 'src/app/demo/api/models';
import { OrderController, ProductController } from 'src/app/services/fastfood.service';
import { OrderHubService } from 'src/app/services/order-hub.service';
import { eOrderState } from 'src/app/enums/eOrderState';

type DateFilter = 'today' | 'week' | 'month' | 'all' | 'custom';

@Component({
    selector: 'app-order',
    templateUrl: './order.component.html',
    styleUrls: ['./order.component.scss'],
    providers: [MessageService]
})
export class OrderComponent implements OnInit, OnDestroy {
    @Input() productsInCart: CartItem[] = [];
    @Output() backToMenu = new EventEmitter<void>();
    products: Product[] = [];
    categories: string[] = [];
    productSizes: string[] = [];
    selectedProductSize: string;
    selectedProduct: Product;
    selectedProductName: string;
    totalPrice: number;
    order: Order;
    orders: Order[] = [];
    filteredOrders: Order[] = [];
    expandedRows: { [orderId: string]: boolean } = {};
    deleteOrderDialog: boolean = false;
    orderToDelete: Order | null = null;
    private hubSubscriptions: Subscription[] = [];

    dateFilterOptions: SelectItem[] = [
        { label: 'Danas', value: 'today' },
        { label: 'Ova nedelja', value: 'week' },
        { label: 'Ovaj mesec', value: 'month' },
        { label: 'Sve', value: 'all' },
        { label: 'Prilagođeni period', value: 'custom' }
    ];
    selectedDateFilter: DateFilter = 'today';
    customDateRange: Date[] | null = null;

    statusFilterOptions: SelectItem[] = [
        { label: 'Sve', value: null },
        { label: 'Kreirano', value: eOrderState.Created },
        { label: 'Spremno', value: eOrderState.Ready },
        { label: 'Završeno', value: eOrderState.Completed },
        { label: 'Otkazano', value: eOrderState.Cancelled }
    ];
    selectedStatus: eOrderState | null = null;

    globalFilterText: string = '';

    constructor(private productController: ProductController, private orderController: OrderController, private messageService: MessageService, private orderHubService: OrderHubService) { }

    ngOnInit() {
        this.orderController.GetAllOrders().subscribe({
            next: result => {
                this.orders = result;
                this.applyFilters();
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING ORDERS LOADING', life: 3000 });
            }
        });

        this.getTotalPrice();

        this.order = {};

        this.orderHubService.connect();
        this.hubSubscriptions.push(
            this.orderHubService.orderCreated$.subscribe(order => this.onOrderCreated(order)),
            this.orderHubService.orderStateChanged$.subscribe(order => this.onOrderStateChanged(order))
        );
    }

    ngOnDestroy() {
        this.hubSubscriptions.forEach(subscription => subscription.unsubscribe());
    }

    onOrderCreated(order: Order) {
        if (!order.id || this.orders.some(o => o.id === order.id)) {
            return;
        }

        this.orders = [order, ...this.orders];
        this.applyFilters();
    }

    onOrderStateChanged(order: Order) {
        if (!order.id) {
            return;
        }

        const index = this.orders.findIndex(o => o.id === order.id);
        if (index === -1) {
            this.orders = [order, ...this.orders];
        } else {
            this.orders = this.orders.map(o => o.id === order.id ? order : o);
        }
        this.applyFilters();
    }

    getTotalPrice() {
        this.totalPrice = 0;

        this.productsInCart.forEach(p => {
            this.totalPrice += p.price;
        });
    }

    private getDateRange(filter: DateFilter): { start: Date; end: Date } | null {
        const now = new Date();

        if (filter === 'today') {
            const start = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            const end = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59, 999);
            return { start, end };
        }

        if (filter === 'week') {
            const dayOfWeek = (now.getDay() + 6) % 7; // 0 = Monday
            const start = new Date(now.getFullYear(), now.getMonth(), now.getDate() - dayOfWeek);
            const end = new Date(start.getFullYear(), start.getMonth(), start.getDate() + 6, 23, 59, 59, 999);
            return { start, end };
        }

        if (filter === 'month') {
            const start = new Date(now.getFullYear(), now.getMonth(), 1);
            const end = new Date(now.getFullYear(), now.getMonth() + 1, 0, 23, 59, 59, 999);
            return { start, end };
        }

        if (filter === 'custom' && this.customDateRange?.[0]) {
            const start = new Date(this.customDateRange[0]);
            start.setHours(0, 0, 0, 0);
            const rangeEnd = this.customDateRange[1] ?? this.customDateRange[0];
            const end = new Date(rangeEnd);
            end.setHours(23, 59, 59, 999);
            return { start, end };
        }

        return null;
    }

    applyFilters() {
        let result = this.orders;

        const range = this.getDateRange(this.selectedDateFilter);
        if (range) {
            result = result.filter(o => {
                if (!o.orderReceivedDateTime)
                    return false;

                const received = new Date(o.orderReceivedDateTime).getTime();
                return received >= range.start.getTime() && received <= range.end.getTime();
            });
        }

        if (this.selectedStatus !== null && this.selectedStatus !== undefined) {
            result = result.filter(o => o.state === this.selectedStatus || (o.state as any) === eOrderState[this.selectedStatus]);
        }

        const search = this.globalFilterText.trim().toLowerCase();
        if (search) {
            result = result.filter(o => {
                const orderNumber = String(o.orderNumber ?? o.id ?? '').toLowerCase();
                const stateLabel = this.getStateLabel(o.state).toLowerCase();
                return orderNumber.includes(search) || stateLabel.includes(search);
            });
        }

        this.filteredOrders = result;
    }

    onDateFilterChange() {
        if (this.selectedDateFilter !== 'custom') {
            this.customDateRange = null;
        }
        this.applyFilters();
    }

    onCustomDateRangeChange() {
        if (this.customDateRange?.[0] && this.customDateRange?.[1]) {
            this.applyFilters();
        }
    }

    resetFilters() {
        this.selectedDateFilter = 'today';
        this.customDateRange = null;
        this.selectedStatus = null;
        this.globalFilterText = '';
        this.applyFilters();
    }

    get hasActiveFilters(): boolean {
        return this.selectedDateFilter !== 'today' || this.selectedStatus !== null || !!this.globalFilterText.trim();
    }

    get dateFilterLabel(): string {
        const option = this.dateFilterOptions.find(o => o.value === this.selectedDateFilter);
        return option?.label ?? '';
    }

    get statusFilterLabel(): string {
        const option = this.statusFilterOptions.find(o => o.value === this.selectedStatus);
        return option?.label ?? '';
    }

    onBackToMenu() {
        this.backToMenu.emit();
    }

    removeProductFromCart(product: Product) {
        let index = this.productsInCart.findIndex(p => p.productId == product.id);
        this.productsInCart.splice(index, 1);
        this.getTotalPrice();
    }

    completeOrder() {
        this.order.orderDeliveredDateTime = this.toLocalIsoString(new Date());
        this.order.state = eOrderState.Completed;

        this.orderController.UpdateOrder(this.order)
            .subscribe(result => {
                if (!!result) {
                    this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Order Updated', life: 3000 });
                }
            },
            error => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING ORDER CREATION', life: 3000 });
            });

        this.onBackToMenu();
    }

    deleteOrder(order: Order) {
        this.orderToDelete = order;
        this.deleteOrderDialog = true;
    }

    confirmDeleteOrder() {
        this.deleteOrderDialog = false;

        const order = this.orderToDelete;
        if (!order?.id) {
            return;
        }

        this.orderController.DeleteOrder(order.id).subscribe({
            next: () => {
                this.orders = this.orders.filter(o => o.id !== order.id);
                this.applyFilters();
                delete this.expandedRows[order.id];
                this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Order Deleted', life: 3000 });
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING ORDER DELETION', life: 3000 });
            }
        });

        this.orderToDelete = null;
    }

    onRowExpand(event: { data: Order }) {
        if (event.data.id)
            this.expandedRows[event.data.id] = true;
    }

    onRowCollapse(event: { data: Order }) {
        if (event.data.id)
            delete this.expandedRows[event.data.id];
    }

    hasDeliveredTime(order: Order): boolean {
        if (order.state == eOrderState.Created || order.state == eOrderState.Ready) 
            return false;

        return true;
    }

    getOrderStatusDateTime(order: Order): string | undefined {
        const state = order.state as any;

        if (state === eOrderState.Ready || state === eOrderState[eOrderState.Ready] || state === 'Ready')
            return order.orderReadyDateTime;

        if (state === eOrderState.Completed || state === eOrderState[eOrderState.Completed] || state === 'Completed')
            return order.orderDeliveredDateTime;

        if (state === eOrderState.Cancelled || state === eOrderState[eOrderState.Cancelled] || state === 'Cancelled')
            return order.orderCancelledDateTime;

        return undefined;
    }

    getStateLabel(state: any): string {
        if (typeof state === 'number')
            return eOrderState[state] ?? String(state);

        return state ?? '';
    }

    // Backend stores naive (no timezone) datetimes and the whole app reads them back as local wall-clock time, so send local time here instead of toISOString()'s UTC instant.
    private toLocalIsoString(date: Date): string {
        const pad = (n: number, length = 2) => n.toString().padStart(length, '0');

        return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}.${pad(date.getMilliseconds(), 3)}`;
    }

}