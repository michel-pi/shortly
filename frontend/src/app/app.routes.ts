import { Routes } from '@angular/router';
import { canActivateAuth } from './core/auth.guard';

export const appRoutes: Routes = [
  { path: 'login', loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent) },

  {
    path: '',
    canActivate: [canActivateAuth],
    children: [
      { path: '', pathMatch: 'full', loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'links', loadComponent: () => import('./pages/links/links.component').then(m => m.LinksComponent) },
      { path: 'access-keys', loadComponent: () => import('./pages/access-keys/access-keys.component').then(m => m.AccessKeysComponent) },
      { path: 'user', loadComponent: () => import('./pages/user/user.component').then(m => m.UserComponent) },
    ]
  },

  { path: '**', redirectTo: '' }
];

export const routes = appRoutes;
