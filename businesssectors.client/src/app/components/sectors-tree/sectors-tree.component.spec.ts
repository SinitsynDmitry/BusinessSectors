import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SectorsTreeComponent } from './sectors-tree.component';

describe('SectorsTreeComponent', () => {
  let component: SectorsTreeComponent;
  let fixture: ComponentFixture<SectorsTreeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SectorsTreeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SectorsTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
