import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { ShortLinksApi } from '../../api/short-links.api';

@Component({
  standalone: true,
  selector: 'app-links',
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule
  ],
  template: `
    <div class="toolbar">
      <h2 class="title">Short Links</h2>
      <button mat-flat-button color="primary" (click)="openCreate()">Create</button>
    </div>

    <table mat-table [dataSource]="rows" class="mat-elevation-z1 full">

      <ng-container matColumnDef="shortCode">
        <th mat-header-cell *matHeaderCellDef>Code</th>
        <td mat-cell *matCellDef="let r">{{ r.shortCode }}</td>
      </ng-container>

      <ng-container matColumnDef="targetUrl">
        <th mat-header-cell *matHeaderCellDef>Target</th>
        <td mat-cell *matCellDef="let r">{{ r.targetUrl }}</td>
      </ng-container>

      <ng-container matColumnDef="isActive">
        <th mat-header-cell *matHeaderCellDef>Active</th>
        <td mat-cell *matCellDef="let r">{{ r.isActive ? 'Yes' : 'No' }}</td>
      </ng-container>

      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef>Actions</th>
        <td mat-cell *matCellDef="let r" class="actions-cell">
          <button mat-icon-button (click)="copy(shortUrl(r.shortCode || ''))" aria-label="Copy URL">
            <mat-icon>content_copy</mat-icon>
          </button>
          <button mat-icon-button (click)="toggle(r)" aria-label="Toggle">
            <mat-icon>{{ r.isActive ? 'pause' : 'play_arrow' }}</mat-icon>
          </button>
          <button mat-icon-button color="warn" (click)="remove(r)" aria-label="Delete">
            <mat-icon>delete</mat-icon>
          </button>
        </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayed"></tr>
      <tr mat-row *matRowDef="let row; columns: displayed;"></tr>
    </table>
  `,
  styles: [`
    .toolbar { display:flex; gap:12px; align-items:center; margin:12px 0; }
    .title { flex:1; }

    .full { width: 100%; table-layout: fixed; }

    th.mat-mdc-header-cell, td.mat-mdc-cell { padding: 0 16px; }

    .mat-mdc-row::after, .mat-mdc-footer-row::after { content: none !important; }
    td.mat-mdc-cell, td.mat-mdc-footer-cell { border-bottom: 1px solid rgba(0,0,0,0.12); }
    th.mat-mdc-header-cell { border-bottom: 1px solid rgba(0,0,0,0.12); }

    .mat-column-actions { width: 180px; text-align: right; }
    .actions-cell { display:flex; gap:4px; justify-content:flex-end; align-items:center; }
  `]
})
export class LinksComponent implements OnInit {
  private api = inject(ShortLinksApi);
  private dialog = inject(MatDialog);

  rows: any[] = [];
  displayed = ['shortCode','targetUrl','isActive','actions'];

  ngOnInit(){ this.reload(); }
  reload(){ this.api.list().subscribe(d => this.rows = d); }

  shortUrl(code: string){ return `${location.origin}/r/${code}`; }
  copy(text: string){ navigator.clipboard?.writeText(text); }

  openCreate(){
    const ref = this.dialog.open(CreateLinkDialog, { width: '480px' });
    ref.afterClosed().subscribe(result => {
      if(!result) return;
      this.api.create({ targetUrl: result.targetUrl, isActive: true }).subscribe(created => {
        this.rows = [created, ...this.rows];
      });
    });
  }

  toggle(r: any){
    this.api.update(r.id, { isActive: !r.isActive }).subscribe(x => Object.assign(r, x));
  }

  remove(r: any){
    if(confirm(`Delete ${r.shortCode}?`)) {
      this.api.delete(r.id).subscribe(() => this.reload());
    }
  }
}

@Component({
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  template: `
    <h2 mat-dialog-title>Create Short Link</h2>
    <form [formGroup]="form" (ngSubmit)="submit()" class="form">
      <mat-form-field appearance="outline" class="w">
        <mat-label>Target URL</mat-label>
        <input matInput type="url" formControlName="targetUrl" required>
      </mat-form-field>

      <div class="actions">
        <button mat-stroked-button type="button" (click)="close()">Cancel</button>
        <button mat-flat-button color="primary" [disabled]="form.invalid">Create</button>
      </div>
    </form>
  `,
  styles: [`
    .form { padding: 8px 24px 24px; }
    .w { width: 100%; }
    .actions { display:flex; justify-content:flex-end; gap:8px; margin-top: 8px; }
  `]
})
export class CreateLinkDialog {
  private fb = inject(FormBuilder);
  private ref = inject(MatDialogRef<CreateLinkDialog>);

  form = this.fb.group({
    targetUrl: ['', [Validators.required]]
  });

  close(){ this.ref.close(); }
  submit(){
    if(this.form.invalid) return;
    this.ref.close(this.form.value);
  }
}
