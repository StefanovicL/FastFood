import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AppLayoutModule } from './layout/app.layout.module';

import { NotfoundComponent } from './demo/components/notfound/notfound.component';
import { ErrorComponent } from './demo/components/auth/error/error.component';
import { AccessComponent } from './demo/components/auth/access/access.component';

import { LandingModule } from './demo/components/landing/landing.module';
import { LoginModule } from './demo/components/auth/login/login.module';
import { CartModule } from './demo/components/pages/cart/cart.module';
import { RegisterModule } from './demo/components/auth/register/register.module';
import { OrderModule } from './demo/components/pages/orders/order.module';
import { ActiveOrdersModule } from './demo/components/pages/active-orders/active-orders.module';
import { OrderStatusModule } from './demo/components/pages/order-status/order-status.module';
import { IngredientsModule } from './demo/components/pages/ingredients/ingredients.module';
import { UsersModule } from './demo/components/pages/users/users.module';
import { CrudModule } from './demo/components/pages/crud/crud.module';
import { ListDemoModule } from './demo/components/uikit/list/listdemo.module';

import { IngredientController, OrderController, ProductController, RoleController, UserController } from './services/fastfood.service';

@NgModule({
  declarations: [
    AppComponent,
    NotfoundComponent,
    ErrorComponent,
    AccessComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    AppLayoutModule,
    LandingModule,
    LoginModule,
    CartModule,
    RegisterModule,
    OrderModule,
    ActiveOrdersModule,
    OrderStatusModule,
    IngredientsModule,
    UsersModule,
    CrudModule,
    ListDemoModule
  ],
  providers: [
    UserController,
    ProductController,
    IngredientController,
    OrderController,
    RoleController
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
