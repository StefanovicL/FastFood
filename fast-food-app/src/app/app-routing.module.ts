import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppLayoutComponent } from './layout/app.layout.component';
import { LandingComponent } from './demo/components/landing/landing.component';
import { CartComponent } from './demo/components/pages/cart/cart.component';
import { NotfoundComponent } from './demo/components/notfound/notfound.component';
import { LoginComponent } from './demo/components/auth/login/login.component';
import { ErrorComponent } from './demo/components/auth/error/error.component';
import { AccessComponent } from './demo/components/auth/access/access.component';
import { RegisterComponent } from './demo/components/auth/register/register.component';
import { OrderComponent } from './demo/components/pages/orders/order.component';
import { ActiveOrdersComponent } from './demo/components/pages/active-orders/active-orders.component';
import { OrderStatusComponent } from './demo/components/pages/order-status/order-status.component';
import { IngredientsComponent } from './demo/components/pages/ingredients/ingredients.component';
import { UsersComponent } from './demo/components/pages/users/users.component';
import { ListDemoComponent } from './demo/components/uikit/list/listdemo.component';
import { CrudComponent } from './demo/components/pages/crud/crud.component';
import { RoleGuard } from './demo/components/auth/role.guard';

const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  {
    path: '', component: AppLayoutComponent,
    canActivateChild: [RoleGuard],
    children: [
      { path: 'uikit/list', component: ListDemoComponent },
      { path: 'pages/crud', component: CrudComponent },
      { path: 'pages/cart', component: CartComponent },
      { path: 'pages/order', component: OrderComponent },
      { path: 'pages/active-orders', component: ActiveOrdersComponent },
      { path: 'pages/order-status', component: OrderStatusComponent },
      { path: 'pages/ingredients', component: IngredientsComponent },
      { path: 'pages/users', component: UsersComponent }
    ]
  },
  { path: 'landing', component: LandingComponent, canActivate: [RoleGuard] },
  { path: 'auth/login', component: LoginComponent },
  { path: 'auth/register', component: RegisterComponent },
  { path: 'auth/error', component: ErrorComponent },
  { path: 'auth/access', component: AccessComponent },
  { path: 'notfound', component: NotfoundComponent },
  { path: '**', redirectTo: '/notfound' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
