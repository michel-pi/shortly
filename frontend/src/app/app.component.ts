import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, Router } from '@angular/router';
import { AuthService } from './core/auth.service';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, MatToolbarModule, MatButtonModule, MatIconModule],
  template: `
    <mat-toolbar color="primary">
      <button mat-icon-button routerLink="/"><mat-icon>link</mat-icon></button>
      <span class="ml-2">Shortly Admin</span>
      <span class="spacer"></span>
      <button mat-button (click)="logout()">Logout</button>
    </mat-toolbar>

    <main class="container">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .spacer { flex: 1; }
    mat-toolbar { display: flex; }
    .container { max-width: 1000px; margin: 16px auto; padding: 0 12px; }
    .ml-2 { margin-left: 8px; }
  `]
})
export class AppComponent {
    constructor(private auth: AuthService, private router: Router) {}
    logout() {
    this.auth.logout().subscribe({
        next: () => this.router.navigateByUrl('/login'),
        error: () => this.router.navigateByUrl('/login')
    });
    }
}
