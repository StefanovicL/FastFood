import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../../api/models';
import { eRole } from 'src/app/enums/eRole';
import { parseRole } from './role-access.config';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private static readonly STORAGE_KEY = 'currentUser';

    private isSignedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    private currentUser: BehaviorSubject<User | null> = new BehaviorSubject<User | null>(null);

    constructor(private router: Router) {
        // Restore persisted login state on app start (page reload) so the user isn't kicked back to Login.
        const storedUser = localStorage.getItem(AuthService.STORAGE_KEY);
        if (storedUser) {
            this.currentUser.next(JSON.parse(storedUser));
            this.isSignedIn.next(true);
        }
    }

    public setSignInStatus(isSignedIn: boolean): void {
        this.isSignedIn.next(isSignedIn);
    }

    public getSignInStatus(): Observable<boolean> {
        return this.isSignedIn.asObservable();
    }

    public getCurrentUsername(): Observable<string | null> {
        return this.currentUser.asObservable().pipe(map(user => user?.username ?? null));
    }

    public getCurrentUser(): Observable<User | null> {
        return this.currentUser.asObservable();
    }

    public getCurrentUserValue(): User | null {
        return this.currentUser.value;
    }

    public setCurrentUser(user: User): void {
        this.currentUser.next(user);
        localStorage.setItem(AuthService.STORAGE_KEY, JSON.stringify(user));
    }

    // Centralized role lookup - use this (or hasRole) instead of comparing user.userRoles[0].role.name strings elsewhere.
    public getCurrentRole(): eRole | null {
        return parseRole(this.currentUser.value?.userRoles?.[0]?.role?.name);
    }

    public hasRole(...roles: eRole[]): boolean {
        const role = this.getCurrentRole();
        return role !== null && roles.includes(role);
    }

    public logout(): void {
        this.setSignInStatus(false);
        localStorage.removeItem(AuthService.STORAGE_KEY);
        this.currentUser.next(null);
        this.router.navigate(['/auth/login']);
    }
}