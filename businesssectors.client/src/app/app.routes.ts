import { Routes } from '@angular/router';
import { SectorsComponent } from './components/sectors/sectors.component';

export const routes: Routes = [
  { path: '', component: SectorsComponent },
  // { path: 'other', loadComponent: () => import('./components/other/other.component').then(m => m.OtherComponent) } // для lazy-loading
];
