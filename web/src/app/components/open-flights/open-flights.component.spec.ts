import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenFlightsComponent } from './open-flights.component';

describe('OpenFlightsComponent', () => {
  let component: OpenFlightsComponent;
  let fixture: ComponentFixture<OpenFlightsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OpenFlightsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenFlightsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
