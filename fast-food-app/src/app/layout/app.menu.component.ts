import { Component, OnInit } from '@angular/core';
import { LayoutService } from './service/app.layout.service';
import { AuthService } from '../demo/components/auth/auth.service';
import { isRouteAllowedForRole } from '../demo/components/auth/role-access.config';

@Component({
    selector: 'app-menu',
    templateUrl: './app.menu.component.html'
})
export class AppMenuComponent implements OnInit {

    model: any[] = [];

    constructor(public layoutService: LayoutService, private authService: AuthService) { }

    ngOnInit() {
        this.authService.getCurrentUser().subscribe(() => this.buildModel());
    }

    private buildModel() {
        const role = this.authService.getCurrentRole();
        const allowed = (path: string) => isRouteAllowedForRole(role, path);

        this.model = [
            {
                items: [
                    ...(allowed('uikit/list') ? [{ label: 'Meni', icon: 'pi pi-fw pi-list', routerLink: ['/uikit/list'] }] : [])
                ]
            },
            {
                icon: 'pi pi-fw pi-briefcase',
                items: [
                    ...(allowed('landing') ? [{
                        label: 'Početna',
                        icon: 'pi pi-fw pi-globe',
                        routerLink: ['landing']
                    }] : []),
                    {
                        label: 'Auth',
                        icon: 'pi pi-fw pi-user',
                        items: [
                            {
                                label: 'Login',
                                icon: 'pi pi-fw pi-sign-in',
                                routerLink: ['/auth/login']
                            },
                            {
                                label: 'Register',
                                icon: 'pi pi-fw pi-sign-in',
                                routerLink: ['/auth/register']
                            },
                            {
                                label: 'Error',
                                icon: 'pi pi-fw pi-times-circle',
                                routerLink: ['/auth/error']
                            },
                            {
                                label: 'Access Denied',
                                icon: 'pi pi-fw pi-lock',
                                routerLink: ['/auth/access']
                            }
                        ]
                    },
                    ...(allowed('pages/crud') ? [{
                        label: 'Proizvodi',
                        icon: 'pi pi-fw pi-pencil',
                        routerLink: ['/pages/crud']
                    }] : []),
                    ...(allowed('pages/cart') ? [{
                        label: 'Cart',
                        icon: 'pi pi-fw pi-circle-off',
                        routerLink: ['/pages/cart']
                    }] : []),
                    ...(allowed('pages/active-orders') ? [{
                        label: 'Active Orders',
                        icon: 'pi pi-fw pi-clock',
                        routerLink: ['/pages/active-orders']
                    }] : []),
                    ...(allowed('pages/order-status') ? [{
                        label: 'Order Status',
                        icon: 'pi pi-fw pi-desktop',
                        routerLink: ['/pages/order-status']
                    }] : []),
                    ...(allowed('pages/ingredients') ? [{
                        label: 'Ingredients',
                        icon: 'pi pi-fw pi-box',
                        routerLink: ['/pages/ingredients']
                    }] : []),
                    ...(allowed('pages/users') ? [{
                        label: 'Users',
                        icon: 'pi pi-fw pi-users',
                        routerLink: ['/pages/users']
                    }] : [])
                ]
            }
        ];
    }
}

