import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

import { AuthService } from '../../core/auth.service';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  template: `
    <div class="login-wrap">
      <mat-card class="card">
        <h1>Sign in</h1>

        <form [formGroup]="form" (ngSubmit)="submit()" class="form">
          <mat-form-field appearance="outline" class="w">
            <mat-label>Email</mat-label>
            <input matInput type="email" formControlName="email" autocomplete="email" required>
            @if (form.controls.email.invalid && (form.controls.email.touched || submitted)) {
              <mat-error>Please enter a valid email.</mat-error>
            }
          </mat-form-field>

          <mat-form-field appearance="outline" class="w">
            <mat-label>Password</mat-label>
            <input matInput type="password" formControlName="password" autocomplete="current-password" required>
            @if (form.controls.password.invalid && (form.controls.password.touched || submitted)) {
              <mat-error>Password is required (min 4 for dev).</mat-error>
            }
          </mat-form-field>

          @if (error) {
            <div class="error">{{ error }}</div>
          }

          <button mat-flat-button color="primary" class="w" [disabled]="form.invalid || loading">
            @if (!loading) { Sign in } @else { Signing in… }
          </button>
        </form>
      </mat-card>
    </div>
  `,
  styles: [`
    .login-wrap { min-height: 100vh; display: grid; place-items: center; padding: 24px; }
    .card { width: 100%; max-width: 420px; }
    .form { display: grid; gap: 16px; margin-top: 8px; }
    .w { width: 100%; }
    .error { color: #d32f2f; }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = false;
  submitted = false;
  error: string | null = null;

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    // Für Dev ggf. minLength(4); Backend validiert ohnehin.
    password: ['', [Validators.required, Validators.minLength(4)]],
  });

  submit() {
    this.submitted = true;
    this.error = null;
    if (this.form.invalid || this.loading) return;

    this.loading = true;
    this.auth.login(this.form.value as any).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: () => {
        this.error = 'Invalid email or password.';
        this.loading = false;
      }
    });
  }
}
