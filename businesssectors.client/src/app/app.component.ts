import { Component, OnInit } from '@angular/core';
import { SectorsComponent } from './components/sectors/sectors.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  styleUrl: './app.component.css',
  imports: [
    SectorsComponent
  ]
})
export class AppComponent {

  title = 'Business Sectors';

}
