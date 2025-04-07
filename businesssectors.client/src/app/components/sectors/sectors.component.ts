// sectors.component.ts
import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { SectorsTreeComponent } from '../sectors-tree/sectors-tree.component';

import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { Subject, takeUntil, finalize, switchMap, tap } from 'rxjs';
import { CanDeactivate } from '@angular/router';

import { SectorNode, SectorsService } from '../../../services/sectors.service';
import { User, UserService } from '../../../services/user.service';

@Component({
  selector: 'app-sectors',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    SectorsTreeComponent
  ],
  templateUrl: './sectors.component.html',
  styleUrls: ['./sectors.component.css']
})
export class SectorsComponent implements OnInit, OnDestroy {
  public sectorNodes: SectorNode[] = [];
  selectedIds: number[] = [];
  user?: User;

  // Login state
  isLoggedIn = false;

  // Form setup
  sectorForm = new FormGroup({
    name: new FormControl('', [Validators.required])
  });

  get nameFormControl() {
    return this.sectorForm.get('name') as FormControl;
  }

  // Loading state indicators
  isLoading = false;
  isSaving = false;

  // Configuration options with default values
  selectChildrenWithParent = true;

  // Form state tracking
  initialFormValue: any = null;
  private destroy$ = new Subject<void>();

  constructor(
    private sectorsService: SectorsService,
    private userSectorsService: UserService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    // No automatic loading - wait for login
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Toggle login state and load/clear sectors
  toggleLoginState() {
    if (this.isLoggedIn) {
      // Handle logout
      this.handleLogout();
    } else {
      // Handle login - validate name first
      if (this.nameFormControl.invalid) {
        this.nameFormControl.markAsTouched();
        this.showError('Please enter your name');
        return;
      }

      // Proceed with login
      this.handleLogin();
    }
  }

  handleLogin() {
    const username = this.nameFormControl.value;

    this.userSectorsService.getUserSectors(username).pipe(
      takeUntil(this.destroy$),

      tap(data => this.user = data),

      switchMap(() => this.sectorsService.getSectorNodes(this.user!.id)),
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (result) => {
        this.sectorNodes = result;

        this.isLoggedIn = true;
        this.nameFormControl.disable(); 

        this.storeInitialFormState();
        this.showSuccess(`Welcome, ${username}!`);
      },
      error: (error) => {
        console.error(error);
        this.showError('Failed to load data. Please try again later.');
      }
    });
  }

  handleLogout() {
    // Show confirmation dialog if there are unsaved changes
    if (this.isFormDirty()) {
      const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        data: {
          title: 'Unsaved Changes',
          message: 'You have unsaved changes. Are you sure you want to log out?',
          confirmButtonText: 'Log Out',
          cancelButtonText: 'Cancel'
        }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.performLogout();
        }
      });
    } else {
      this.performLogout();
    }
  }

  performLogout() {
    // Clear sectors and reset login state
    this.isLoggedIn = false;
    this.sectorNodes = [];
    this.selectedIds = [];
    this.sectorForm.enable(); // Re-enable the name field
    this.initialFormValue = null;
    this.showSuccess('You have been logged out');
  }

  loadSectors(userId: number) {
    this.isLoading = true;
    this.sectorsService.getSectorNodes(userId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isLoading = false)
      )
      .subscribe({
        next: (result) => {
          this.sectorNodes = result;

          // Update login state
          this.isLoggedIn = true;
          this.nameFormControl.disable(); // Disable the name field

          // Store initial form state after loading data
          this.storeInitialFormState();

          this.showSuccess(`Welcome, ${this.user!.name}!`);
        },
        error: (error) => {
          console.error(error);
          this.showError('Failed to load sectors. Please try again later.');
        }
      });
  }

  loadUser(userName: string) {
    this.userSectorsService.getUserSectors(userName).subscribe({
      next: (data) => this.user = data,
      error: (err) => console.error('Failed to load user sectors', err)
    });
  }

  storeInitialFormState() {
    this.initialFormValue = {
      name: this.nameFormControl.value,
      selectedIds: [...this.selectedIds]
    };
  }

  onSelectedIdsChange(ids: number[]) {
    this.selectedIds = ids;
    console.log('Selected IDs:', ids);
  }

  onRestore() {
    // If form is not dirty, no need for confirmation
    if (!this.isFormDirty()) {
      this.loadSectors(this.user!.id);
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Confirm Restore',
        message: 'You will lose all your changes. Are you sure?',
        confirmButtonText: 'Restore',
        cancelButtonText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadSectors(this.user!.id);
      }
    });
  }

  onSave() {
    // Validate form
    if (this.sectorForm.invalid) {
      this.sectorForm.markAllAsTouched();
      this.showError('Please fill out all required fields');
      return;
    }

    // Check if any sectors are selected
    if (this.selectedIds.length === 0) {
      this.showError('Please select at least one sector');
      return;
    }
    console.log('User sectors loaded:', this.user);
    const data = {
      id: this.user!.id,
      selectedSectorIds: this.selectedIds.join(',')
    };

    // Call service to save data 
    this.isSaving = true;
    this.userSectorsService.updateUserSectors(data.id, data.selectedSectorIds)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isSaving = false)
      )
      .subscribe({
        next: (response) => {
          console.log('Saved successfully', response);
          this.showSuccess('Your selections have been saved successfully');
          // Update initial form state after successful save
          this.storeInitialFormState();
        },
        error: (error) => {
          console.error('Error saving data', error);
          this.showError('Failed to save your selections. Please try again later.');
        }
      });
  }

  // Prevent navigation if form is dirty
  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload($event: any) {
    if (this.isLoggedIn && this.isFormDirty()) {
      $event.returnValue = 'You have unsaved changes. Are you sure you want to leave?';
    }
  }

  // Method for CanDeactivate guard
  canDeactivate(): boolean | Promise<boolean> {
    if (!this.isLoggedIn || !this.isFormDirty()) {
      return true;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Unsaved Changes',
        message: 'You have unsaved changes. Are you sure you want to leave this page?',
        confirmButtonText: 'Leave',
        cancelButtonText: 'Stay'
      }
    });

    return dialogRef.afterClosed().toPromise();
  }

  isFormDirty(): boolean {
    if (!this.initialFormValue) return false;

    const currentValue = {
      name: this.nameFormControl.value,
      selectedIds: [...this.selectedIds]
    };

    return this.initialFormValue.name !== currentValue.name ||
      !this.arraysEqual(this.initialFormValue.selectedIds, currentValue.selectedIds);
  }

  private arraysEqual(a: number[], b: number[]): boolean {
    if (a.length !== b.length) return false;
    const sortedA = [...a].sort();
    const sortedB = [...b].sort();
    return sortedA.every((val, idx) => val === sortedB[idx]);
  }

  // Helper methods for user feedback
  showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['success-snackbar']
    });
  }

  showError(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }
}
