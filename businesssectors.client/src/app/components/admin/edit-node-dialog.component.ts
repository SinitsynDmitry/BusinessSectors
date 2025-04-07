import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-edit-node-dialog',
  template: `
    <h2 mat-dialog-title>Edit Sector Name</h2>
    <mat-dialog-content>
      <mat-form-field appearance="fill" style="width: 100%">
        <mat-label>Name</mat-label>
        <input matInput [(ngModel)]="data.name" required>
      </mat-form-field>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onNoClick()">Cancel</button>
      <button mat-button [mat-dialog-close]="data.name" [disabled]="!data.name" cdkFocusInitial>Save</button>
    </mat-dialog-actions>
  `,
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule
  ]
})
export class EditNodeDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<EditNodeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { name: string }
  ) { }

  onNoClick(): void {
    this.dialogRef.close();
  }
}
