import { Component, OnInit } from '@angular/core';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { AuthService } from '../auth.service';
import { UserController } from 'src/app/services/fastfood.service';
import { User } from 'src/app/demo/api/models';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { getDefaultRouteForRole, parseRole } from '../role-access.config';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styles: [`
        :host ::ng-deep .pi-eye,
        :host ::ng-deep .pi-eye-slash {
            transform:scale(1.6);
            margin-right: 1rem;
            color: var(--primary-color) !important;
        }
    `],
    providers: [MessageService]
})
export class LoginComponent implements OnInit {
    valCheck: string[] = ['remember'];
    password!: string;
    username!: string;
    users: User[] = [];

    constructor(
        public layoutService: LayoutService, 
        private authService: AuthService, 
        private userController: UserController, 
        private messageService: MessageService,
        private router: Router) { }

    ngOnInit(): void {
        this.userController.GetAllUsers().subscribe({
            next: result => {
                this.users = result ?? [];
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING USERS LOADING', life: 3000 });
            }
        });
    }

    onSignIn(): void {
        let user = this.users.find(u => u.username == this.username);
        if (!!user && user.password === this.password) {
            this.authService.setSignInStatus(true);
            this.authService.setCurrentUser(user);
            const role = parseRole(user.userRoles?.[0]?.role?.name);
            this.router.navigate(['/' + getDefaultRouteForRole(role)]);
        } else {
            this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'USERNAME OR PASSWORD IS INCORRECT.', life: 3000 });
        }
    }
}
