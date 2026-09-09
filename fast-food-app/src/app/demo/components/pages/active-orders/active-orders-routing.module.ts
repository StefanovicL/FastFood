import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ActiveOrdersComponent } from './active-orders.component';

@NgModule({
    imports: [RouterModule.forChild([
        { path: '', component: ActiveOrdersComponent }
    ])],
    exports: [RouterModule]
})
export class ActiveOrdersRoutingModule { }
