import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { LayoutService } from "./service/app.layout.service";
import { Router } from '@angular/router';
import { AuthService } from '../demo/components/auth/auth.service';
import { isRouteAllowedForRole } from '../demo/components/auth/role-access.config';

@Component({
    selector: 'app-topbar',
    templateUrl: './app.topbar.component.html'
})
export class AppTopBarComponent implements OnInit {
    items!: MenuItem[];
    isSignedIn: boolean = false;
    username: string = '';

    constructor(public layoutService: LayoutService, public router: Router, private authService: AuthService) { }

    ngOnInit(): void {
        this.authService.getSignInStatus().subscribe(result => {
            this.isSignedIn = result;
        });

        this.authService.getCurrentUsername().subscribe(result => {
            this.username = result;
        });
    }

    canAccess(path: string): boolean {
        return isRouteAllowedForRole(this.authService.getCurrentRole(), path);
    }

    logout() {
        this.authService.logout();
    }
}
