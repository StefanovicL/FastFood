import { Component, OnInit } from '@angular/core';
import { MessageService, SelectItem } from 'primeng/api';
import { Table } from 'primeng/table';
import { Role, User } from 'src/app/demo/api/models';
import { RoleController, UserController } from 'src/app/services/fastfood.service';

@Component({
  templateUrl: './users.component.html',
  providers: [MessageService]
})
export class UsersComponent implements OnInit {

  userDialog: boolean = false;
  deleteUserDialog: boolean = false;

  users: User[] = [];
  user: User = {};
  submitted: boolean = false;

  roles: Role[] = [];
  roleOptions: SelectItem[] = [];

  constructor(
    private messageService: MessageService,
    private userController: UserController,
    private roleController: RoleController
  ) { }

  ngOnInit() {
    this.initializeRoles();
    this.initializeUsers();
  }

  initializeUsers() {
    this.userController.GetAllUsers().subscribe({
      next: (result) => {
        this.users = result ?? [];
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR LOADING USERS', life: 3000 });
      }
    });
  }

  initializeRoles() {
    this.roleController.GetAllRoles().subscribe({
      next: (result) => {
        this.roles = result ?? [];
        this.roleOptions = this.roles.map(r => ({ label: r.name, value: r.id }));
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR LOADING ROLES', life: 3000 });
      }
    });
  }

  roleNames(user: User): string {
    return (user.userRoles ?? [])
      .map(ur => ur.role?.name)
      .filter(name => !!name)
      .join(', ');
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }

  openNew() {
    this.user = {};
    this.submitted = false;
    this.userDialog = true;
  }

  editUser(user: User) {
    this.user = { ...user, roleId: user.userRoles?.[0]?.role?.id, password: '' };
    this.userDialog = true;
  }

  hideDialog() {
    this.userDialog = false;
    this.submitted = false;
  }

  deleteUser(user: User) {
    this.user = { ...user };
    this.deleteUserDialog = true;
  }

  confirmDelete() {
    this.deleteUserDialog = false;

    const id = this.user.id;
    if (!id)
      return;

    this.userController.DeleteUser(id).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'User Deleted', life: 3000 });
        this.initializeUsers();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Unsuccessful', detail: 'ERROR DURING USER DELETION', life: 3000 });
      }
    });

    this.user = {};
  }

  saveUser() {
    this.submitted = true;

    const isUpdate = !!this.user.id;

    if (!this.user.username || this.user.roleId == null || (!isUpdate && !this.user.password)) {
      this.messageService.add({ severity: 'warn', summary: 'Missing', detail: 'Username, password and role are required', life: 3000 });
      return;
    }

    // Backend rejects a nested userRoles graph (User field required); send only scalar fields.
    const payload: User = {
      id: this.user.id,
      username: this.user.username,
      password: this.user.password,
      roleId: this.user.roleId
    };

    const req$ = isUpdate
      ? this.userController.UpdateUser(payload)
      : this.userController.CreateUser(payload);

    req$.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Successful',
          detail: isUpdate ? 'User Updated' : 'User Created',
          life: 3000
        });

        this.userDialog = false;
        this.initializeUsers();
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Unsuccessful',
          detail: isUpdate ? 'ERROR DURING USER UPDATE' : 'ERROR DURING USER CREATION',
          life: 3000
        });
      }
    });
  }
}
