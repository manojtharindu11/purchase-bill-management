import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/login/login.component';
import { PurchaseBillComponent } from './features/purchase-bill/purchase-bill.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  {
    path: 'purchase-bill',
    component: PurchaseBillComponent,
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: 'login' },
];

