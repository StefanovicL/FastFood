import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageService, SelectItem } from 'primeng/api';
import { Order, CartItem } from 'src/app/demo/api/models';
import { eProductSize } from 'src/app/enums/eProductSize';
import { OrderController } from 'src/app/services/fastfood.service';
import { DataView } from 'primeng/dataview';
import { eOrderState } from 'src/app/enums/eOrderState';
import { ProductImageService } from 'src/app/demo/service/product-image.service';

@Component({
    selector: 'app-cart',
    templateUrl: './cart.component.html',
    styleUrls: ['./cart.component.scss'],
    providers: [MessageService]
})
export class CartComponent implements OnInit {
    @Input() productsInCart: CartItem[] = [];
    @Output() backToMenu = new EventEmitter<void>();
    sortOptions: SelectItem[] = [];
    sortOrder: number = 0;
    sortField: string = '';
    productSizes: string[] = [];
    totalPrice: number;
    order: Order;
    orderConfirmationDialog: boolean = false;
    confirmedOrderNumber: number | undefined;

    constructor(private orderController: OrderController, private messageService: MessageService, public productImageService: ProductImageService) { }

    ngOnInit() {
        this.productSizes = Object.keys(eProductSize).filter(key => isNaN(Number(key)));
    
        this.sortOptions = [
            { label: 'Cena: opadajuće', value: '!price' },
            { label: 'Cena: rastuće', value: 'price' }
        ];
        
        this.getTotalPrice();

        this.order = {};
    }
    
    getTotalPrice() {
        this.totalPrice = this.productsInCart.reduce((sum, item) => {
            return sum + (item.price * item.quantity);
        }, 0);
    }

    onSortChange(event: any) {
        const value = event.value;

        if (value.indexOf('!') === 0) {
            this.sortOrder = -1;
            this.sortField = value.substring(1, value.length);
        } else {
            this.sortOrder = 1;
            this.sortField = value;
        }
    }

    onFilter(dv: DataView, event: Event) {
        dv.filter((event.target as HTMLInputElement).value);
    }

    onBackToMenu() {
        this.backToMenu.emit();
    }

    onOrderConfirmationOk() {
        this.orderConfirmationDialog = false;
        this.onBackToMenu();
    }

    removeProductFromCart(item: CartItem) {
        const index = this.productsInCart.findIndex(p => p.productVariantId === item.productVariantId);
        if (index !== -1) {
            this.productsInCart.splice(index, 1);
            this.getTotalPrice();
        }
    }

    // Backend stores naive (no timezone) datetimes and the whole app reads them back as local wall-clock time, so send local time here instead of toISOString()'s UTC instant.
    private toLocalIsoString(date: Date): string {
        const pad = (n: number, length = 2) => n.toString().padStart(length, '0');

        return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}.${pad(date.getMilliseconds(), 3)}`;
    }

    createOrder() {
        const now = this.toLocalIsoString(new Date());

        const order: Order = {
            orderReceivedDateTime: now,
            orderDeliveredDateTime: now,
            state: eOrderState.Created,
            totalPrice: this.totalPrice,
            productVariantOrders: this.productsInCart.map(ci => ({
                productVariant_FK: ci.productVariantId,
                quantity: ci.quantity,
                unitPrice: ci.price
            }))
        };

        this.orderController.CreateOrder(order).subscribe({
            next: (createdOrder: Order) => {
                this.confirmedOrderNumber = createdOrder?.orderNumber ?? createdOrder?.id;
                this.productsInCart.splice(0, this.productsInCart.length);
                this.totalPrice = 0;
                this.orderConfirmationDialog = true;
            },
            error: (error) => {
                const detail = error?.error?.message || 'ERROR DURING ORDER CREATION';
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail, life: 6000 });
            }
        });
    }

    increaseQuantity(item: CartItem) {
        item.quantity += 1;
        this.getTotalPrice();
    }

    decreaseQuantity(item: CartItem) {
        if (item.quantity > 1) {
            item.quantity -= 1;
        } else {
            this.removeProductFromCart(item);
            return;
        }
        this.getTotalPrice();
    }
}