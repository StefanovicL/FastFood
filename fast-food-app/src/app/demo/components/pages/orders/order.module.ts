import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderRoutingModule } from './order-routing.module';
import { OrderComponent } from './order.component';
import { TableModule } from 'primeng/table';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { PickListModule } from 'primeng/picklist';
import { OrderListModule } from 'primeng/orderlist';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';

@NgModule({
    imports: [
        CommonModule,
        OrderRoutingModule,
        TableModule,
        CalendarModule,
        DropdownModule,
        ButtonModule,
		FormsModule,
		PickListModule,
		OrderListModule,
		InputTextModule,
        ToastModule,
        DialogModule
    ],
    declarations: [OrderComponent],
    exports: [OrderComponent]
})
export class OrderModule { }
