import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CloseFlightsComponent } from './close-flights.component';

describe('CloseFlightsComponent', () => {
  let component: CloseFlightsComponent;
  let fixture: ComponentFixture<CloseFlightsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CloseFlightsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CloseFlightsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
