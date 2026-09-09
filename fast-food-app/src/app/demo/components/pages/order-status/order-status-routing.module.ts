import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { OrderStatusComponent } from './order-status.component';

@NgModule({
    imports: [RouterModule.forChild([
        { path: '', component: OrderStatusComponent }
    ])],
    exports: [RouterModule]
})
export class OrderStatusRoutingModule { }
