import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/auth.service';

@Component({
  standalone: true,
  selector: 'app-user',
  imports: [CommonModule, MatCardModule, MatListModule, MatChipsModule, MatIconModule],
  template: `
    @if (auth.user$ | async; as user) {
      <mat-card class="card">
        <h2 class="title"><mat-icon>person</mat-icon> User Profile</h2>
        <mat-list>
          <mat-list-item>
            <div matListItemTitle>Name</div>
            <div matListItemLine>{{ user.name }}</div>
          </mat-list-item>

          <mat-list-item>
            <div matListItemTitle>Email</div>
            <div matListItemLine>{{ user.email }}</div>
          </mat-list-item>

          <mat-list-item>
            <div matListItemTitle>User ID</div>
            <div matListItemLine>{{ user.id }}</div>
          </mat-list-item>

          <mat-list-item>
            <div matListItemTitle>Roles</div>
            <div matListItemLine>
              @if (user.roles?.length) {
                <div class="chips">
                  @for (r of user.roles; track r) {
                    <mat-chip>{{ r }}</mat-chip>
                  }
                </div>
              } @else {
                <span>—</span>
              }
            </div>
          </mat-list-item>
        </mat-list>
      </mat-card>
    } @else {
      <div class="loading">Loading…</div>
    }
  `,
  styles: [`
    .card { max-width: 720px; }
    .title { display: flex; align-items: center; gap: 8px; margin: 8px 0 16px; }
    .chips { display: flex; gap: 6px; flex-wrap: wrap; }
    .loading { padding: 24px; opacity: 0.7; }
  `]
})
export class UserComponent {
  auth = inject(AuthService);
}
