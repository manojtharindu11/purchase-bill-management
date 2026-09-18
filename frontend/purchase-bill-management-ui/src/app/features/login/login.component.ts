import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { LoadingService } from '../../core/services/loading.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  readonly loading = inject(LoadingService);

  readonly errorMessage = signal<string | null>(null);
  readonly showPassword = signal(false);

  readonly form = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(4)]],
  });

  get username() {
    return this.form.controls.username;
  }

  get password() {
    return this.form.controls.password;
  }

  togglePasswordVisibility(): void {
    this.showPassword.update((visible) => !visible);
  }

  submit(): void {
    this.errorMessage.set(null);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.auth
      .login(this.form.getRawValue())
      .pipe(finalize(() => undefined))
      .subscribe({
        next: async () => {
          const navigated = await this.router.navigate(['/purchase-bill']);
          if (!navigated) {
            this.errorMessage.set('Could not open the purchase bill page. Please try again.');
          }
        },
        error: (error: HttpErrorResponse) => {
          const serverMessage =
            (error.error as { message?: string } | null)?.message ??
            (typeof error.error === 'string' ? error.error : null);
          this.errorMessage.set(
            serverMessage ?? 'Login failed. Please check your email and password.',
          );
        },
      });
  }
}
