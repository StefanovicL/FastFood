import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActiveOrdersRoutingModule } from './active-orders-routing.module';
import { ActiveOrdersComponent } from './active-orders.component';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { ToastModule } from 'primeng/toast';

@NgModule({
    imports: [
        CommonModule,
        ActiveOrdersRoutingModule,
        TableModule,
        ButtonModule,
        RippleModule,
        ToastModule
    ],
    declarations: [ActiveOrdersComponent]
})
export class ActiveOrdersModule { }
