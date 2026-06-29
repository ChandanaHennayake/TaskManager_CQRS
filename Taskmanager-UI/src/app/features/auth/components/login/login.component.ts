import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  isLoginMode = true;
  loading = false;

  message = '';
  errorMessage = '';

  readonly loginForm = this.fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required],
  });

  readonly registerForm = this.fb.nonNullable.group({
    username: [
      '',
      [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
      ],
    ],
    email: ['', [Validators.required, Validators.email]],
    password: [
      '',
      [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(
          /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()_+\-=\[\]{};':"\\|,.<>/?]).{8,}$/
        ),
      ],
    ],
  });

  // Easy access in template
  get lf() {
    return this.loginForm.controls;
  }

  get rf() {
    return this.registerForm.controls;
  }

  toggleMode(): void {
    this.isLoginMode = !this.isLoginMode;

    this.clearMessages();

    this.loginForm.reset();
    this.registerForm.reset();
  }

  login(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.clearMessages();

    this.authService
      .login(this.loginForm.getRawValue())
      .pipe(
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/tasks']);
        },
        error: (error) => {
          this.errorMessage =
            error.error?.message ??
            'Invalid username or password.';
        },
      });
  }

  register(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.clearMessages();

    this.authService
      .register(this.registerForm.getRawValue())
      .pipe(
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: () => {
          const username = this.registerForm.controls.username.value;

          this.isLoginMode = true;

          this.loginForm.patchValue({
            username,
            password: '',
          });

          this.registerForm.reset();

          this.message =
            'Registration successful. Please sign in.';
        },
        error: (error) => {
          this.errorMessage =
            error.error?.message ??
            'Registration failed.';
        },
      });
  }

  private clearMessages(): void {
    this.message = '';
    this.errorMessage = '';
  }
}