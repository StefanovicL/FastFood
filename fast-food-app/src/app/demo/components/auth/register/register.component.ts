import { Component, OnInit } from '@angular/core';
import { MessageService, SelectItem } from 'primeng/api';
import { Role, User } from 'src/app/demo/api/models';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { RoleController, UserController } from 'src/app/services/fastfood.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styles: [`
        :host ::ng-deep .pi-eye,
        :host ::ng-deep .pi-eye-slash {
            transform:scale(1.6);
            margin-right: 1rem;
            color: var(--primary-color) !important;
        }  
        :host ::ng-deep .p-dropdown {
            width: 10rem;
        }
    `],
    providers: [MessageService]
})
export class RegisterComponent implements OnInit {

    // valCheck: string[] = ['remember'];
    password1: string = '';
    password2: string = '';
    username: string = '';
    // user: User;
    roles: SelectItem[] = [];
    // selectedRole: string;
    selectedRoleId: number | null = null;

    constructor(public layoutService: LayoutService, private userController: UserController, private messageService: MessageService, private roleController: RoleController) { }

    ngOnInit() {
        this.roleController.GetAllRoles().subscribe({
            next: (res) => {
                this.roles = (res ?? []).map(r => ({ label: r.name, value: r.id }));
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'Failed to load roles', life: 3000 });
            }
        });

        // this.user = {};
    }

    register() {
        if (!this.selectedRoleId) {
            this.messageService.add({ severity: 'warn', summary: 'Missing', detail: 'Please select a role', life: 3000 });
            return;
        }

        if (!this.username || !this.password1 || this.password1 !== this.password2) {
            this.messageService.add({ severity: 'warn', summary: 'Invalid', detail: 'Check username/password', life: 3000 });
            return;
        }

        const payload = {
            username: this.username,
            password: this.password1,
            roleId: this.selectedRoleId
        };

        this.userController.CreateUser(payload).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'User created', life: 3000 });
                this.username = '';
                this.password1 = '';
                this.password2 = '';
                this.selectedRoleId = null;
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING USER CREATION', life: 3000 });
            }
        });
    }

    onRoleChange() {
        // console.log(this.user);
    }
}
