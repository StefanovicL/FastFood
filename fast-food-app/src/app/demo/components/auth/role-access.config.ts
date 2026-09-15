import { eRole } from 'src/app/enums/eRole';

// Central place defining which route paths (matching Angular route "path", not the full URL) each role may access,
// and where each role lands right after login. Admin bypasses this map entirely (sees everything, no restrictions).
// Add a new page here (and to the route path list) instead of comparing role strings anywhere else in the app.
const ROLE_ALLOWED_ROUTES: Record<eRole, string[]> = {
    [eRole.Admin]: [],
    [eRole.Kupac]: ['landing', 'uikit/list', 'pages/cart'],
    [eRole.Radnik]: ['uikit/list', 'pages/cart', 'pages/order', 'pages/order-status', 'pages/ingredients'],
    [eRole.Kuvar]: ['pages/active-orders', 'pages/ingredients'],
    [eRole.Pregled]: ['pages/order-status']
};

const ROLE_DEFAULT_ROUTE: Record<eRole, string> = {
    [eRole.Admin]: 'uikit/list',
    [eRole.Kupac]: 'landing',
    [eRole.Radnik]: 'uikit/list',
    [eRole.Kuvar]: 'pages/active-orders',
    [eRole.Pregled]: 'pages/order-status'
};

// Parses the Role.Name string coming from the API into the eRole enum, returning null if it doesn't match a known role.
export function parseRole(roleName: string | null | undefined): eRole | null {
    return roleName && (Object.values(eRole) as string[]).includes(roleName) ? (roleName as eRole) : null;
}

export function isRouteAllowedForRole(role: eRole | null, path: string): boolean {
    if (!role) {
        return false;
    }
    if (role === eRole.Admin) {
        return true;
    }
    return ROLE_ALLOWED_ROUTES[role].includes(path);
}

export function getDefaultRouteForRole(role: eRole | null): string {
    return role ? ROLE_DEFAULT_ROUTE[role] : 'auth/login';
}
