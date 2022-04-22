import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PromosPageComponent } from './promos-page.component';

describe('PromosPageComponent', () => {
  let component: PromosPageComponent;
  let fixture: ComponentFixture<PromosPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PromosPageComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PromosPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
