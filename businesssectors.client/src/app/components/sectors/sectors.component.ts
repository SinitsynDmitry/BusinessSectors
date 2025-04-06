import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { SectorsTreeComponent } from '../sectors-tree/sectors-tree.component';
import { Sector, SectorsService } from '../../../services/sectors.service';


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
    SectorsTreeComponent
  ],
  templateUrl: './sectors.component.html',
  styleUrls: ['./sectors.component.css']
})
export class SectorsComponent implements OnInit {
  public sectors: Sector[] = [];
  sectorNodes: any[] = [];
  selectedIds: number[] = [];
  nameFormControl = new FormControl('', [Validators.required]);
  // Configuration options with default values
  selectChildrenWithParent = true;

  constructor(private sectorsService: SectorsService) { }

  ngOnInit() {
    this.getSectors("test");
  }

  getSectors(userName:string) {
    this.sectorsService.getSectors(userName).subscribe({
      next: (result) => {
        this.sectors = result;
        this.sectorNodes = this.sectorsService.convertSectorsToSectorNodes(this.sectors);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  onSelectedIdsChange(ids: number[]) {
    this.selectedIds = ids;
    console.log('Selected IDs:', ids);

  }

  onRestore() {

    this.selectedIds = [];


    this.nameFormControl.reset();

  }

  onSave() {
    // Validate form
    if (this.nameFormControl.invalid) {
      this.nameFormControl.markAsTouched();
      return;
    }

    if (this.selectedIds.length === 0) {
      // Show an error or alert that selections are required
      console.error('Please select at least one sector');
      return;
    }

    const data = {
      name: this.nameFormControl.value!,
      selectedSectorIds: this.selectedIds
    };

    // Call your service method to save data
    this.sectorsService.saveSectorSelections(data).subscribe({
      next: (response) => {
        console.log('Saved successfully', response);
        // Show success message or navigate to another page
      },
      error: (error) => {
        console.error('Error saving data', error);
        // Show error message
      }
    });
  }

}
