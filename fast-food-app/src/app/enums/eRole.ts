// Mirrors the Role.Name values stored in the database (Role table) - never compare raw role-name strings elsewhere, use this enum instead.
export enum eRole {
    Admin = 'Admin',
    Kupac = 'Kupac',
    Radnik = 'Radnik',
    Kuvar = 'Kuvar',
    Pregled = 'Pregled'
}
