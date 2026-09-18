import { inject, Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { API_BASE_URL, API_PREFIX } from '../config/api.config';
import { LoginRequest, LoginResponse } from '../models/auth.models';

const TOKEN_KEY = 'pbm_token';
const USER_KEY = 'pbm_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly _user = signal<LoginResponse | null>(this.readStoredUser());
  readonly currentUser = this._user.asReadonly();
  readonly isAuthenticated = computed(() => !!this._token() && !!this._user());

  login(request: LoginRequest) {
    return this.http.post<LoginResponse>(`${API_BASE_URL}${API_PREFIX}/auth/login`, request).pipe(
      tap((response) => {
        localStorage.setItem(TOKEN_KEY, response.token);
        this._token.set(response.token);
        localStorage.setItem(
          USER_KEY,
          JSON.stringify({
            userCode: response.userCode,
            userDisplayName: response.userDisplayName,
            email: response.email,
          }),
        );
        this._user.set(response);
      }),
    );
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._token.set(null);
    this._user.set(null);
    void this.router.navigate(['/login']);
  }

  private readStoredUser(): LoginResponse | null {
    const raw = localStorage.getItem(USER_KEY);
    const token = localStorage.getItem(TOKEN_KEY);
    if (!raw || !token) {
      return null;
    }
    try {
      const parsed = JSON.parse(raw) as Pick<
        LoginResponse,
        'userCode' | 'userDisplayName' | 'email'
      >;
      return { token, locations: [], ...parsed };
    } catch {
      return null;
    }
  }
}
