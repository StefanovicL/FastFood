import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ListDemoRoutingModule } from './listdemo-routing.module';
import { ListDemoComponent } from './listdemo.component';
import { CartModule } from '../../pages/cart/cart.module';
import { DataViewModule } from 'primeng/dataview';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';

@NgModule({
  imports: [CommonModule, FormsModule, ListDemoRoutingModule, CartModule, DataViewModule, DropdownModule, ButtonModule, RippleModule, InputTextModule, ToastModule],
  declarations: [ListDemoComponent]
})
export class ListDemoModule { }
