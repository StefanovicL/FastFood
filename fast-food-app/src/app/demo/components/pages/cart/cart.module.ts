import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartRoutingModule } from './cart-routing.module';
import { CartComponent } from './cart.component';
import { DataViewModule } from 'primeng/dataview';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { PickListModule } from 'primeng/picklist';
import { OrderListModule } from 'primeng/orderlist';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';

import { EnumFormatPipe } from 'src/app/enum-format.pipe';

@NgModule({
    imports: [
        CommonModule,
        CartRoutingModule,
        DataViewModule,
        DropdownModule,
        ButtonModule,
		FormsModule,
		PickListModule,
		OrderListModule,
		InputTextModule,
        ToastModule,
        DialogModule
    ],
    declarations: [
        CartComponent,
        EnumFormatPipe
    ],
    exports: [
        CartComponent,
        EnumFormatPipe
    ]
})
export class CartModule { }
