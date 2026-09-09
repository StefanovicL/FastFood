import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, Router, UrlTree } from '@angular/router';
import { AuthService } from './auth.service';
import { getDefaultRouteForRole, isRouteAllowedForRole } from './role-access.config';

// Guards both top-level routes (e.g. 'landing') and the AppLayoutComponent's children, preventing direct
// navigation (typed URL, bookmark, etc.) to a page the current user's role isn't allowed to see.
@Injectable({
    providedIn: 'root'
})
export class RoleGuard implements CanActivate, CanActivateChild {

    constructor(private authService: AuthService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
        return this.check(route);
    }

    canActivateChild(route: ActivatedRouteSnapshot): boolean | UrlTree {
        return this.check(route);
    }

    private check(route: ActivatedRouteSnapshot): boolean | UrlTree {
        const path = route.routeConfig?.path;
        if (!path) {
            return true;
        }

        const user = this.authService.getCurrentUserValue();
        if (!user) {
            return this.router.createUrlTree(['/auth/login']);
        }

        const role = this.authService.getCurrentRole();
        if (!isRouteAllowedForRole(role, path)) {
            return this.router.createUrlTree(['/' + getDefaultRouteForRole(role)]);
        }

        return true;
    }
}
