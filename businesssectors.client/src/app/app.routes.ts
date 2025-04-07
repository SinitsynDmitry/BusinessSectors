import { Routes } from '@angular/router';
import { SectorsComponent } from './components/sectors/sectors.component';
import { AdminComponent } from './components/admin/admin.component';

export const routes: Routes = [
  /*{ path: '', component: SectorsComponent },*/
  { path: '', component: SectorsComponent },
  { path: 'admin', component: AdminComponent },
  { path: '**', redirectTo: '' }
  // { path: 'other', loadComponent: () => import('./components/other/other.component').then(m => m.OtherComponent) } // для lazy-loading
];
