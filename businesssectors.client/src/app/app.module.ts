import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SectorsComponent } from './components/sectors/sectors.component';
import { SectorsTreeComponent } from './components/sectors-tree/sectors-tree.component';

@NgModule({
  declarations: [
    AppComponent,
    SectorsComponent,
    SectorsTreeComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
