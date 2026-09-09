import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderStatusRoutingModule } from './order-status-routing.module';
import { OrderStatusComponent } from './order-status.component';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { ToastModule } from 'primeng/toast';

@NgModule({
    imports: [
        CommonModule,
        OrderStatusRoutingModule,
        ButtonModule,
        RippleModule,
        ToastModule
    ],
    declarations: [OrderStatusComponent]
})
export class OrderStatusModule { }
