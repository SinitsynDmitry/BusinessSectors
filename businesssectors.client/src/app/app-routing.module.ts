import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//import { CanDeactivateGuard } from './can-deactivate.guard';
//import { SectorsComponent } from './components/sectors/sectors.component';

const routes: Routes = [
  {
    //path: 'sectors',
    //component: SectorsComponent,
    //canDeactivate: [CanDeactivateGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
