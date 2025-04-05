import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Sector, SectorsService } from '../../../services/sectors.service';


@Component({
  selector: 'app-sectors',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sectors.component.html',
  styleUrl: './sectors.component.css'
})
export class SectorsComponent implements OnInit {
  public sectors: Sector[] = [];

  constructor(private sectorsService: SectorsService) { }

  ngOnInit() {
    this.getSectors("test");
  }

  getSectors(userName:string) {
    this.sectorsService.getSectors(userName).subscribe({
      next: (result) => {
        this.sectors = result;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }
}
