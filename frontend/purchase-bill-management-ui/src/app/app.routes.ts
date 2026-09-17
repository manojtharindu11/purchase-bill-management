import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/login/login.component';
import { PurchaseBillPlaceholderComponent } from './features/purchase-bill/purchase-bill-placeholder.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  {
    path: 'purchase-bill',
    component: PurchaseBillPlaceholderComponent,
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: 'login' },
];

