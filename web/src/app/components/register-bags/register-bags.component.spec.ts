import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterBagsComponent } from './register-bags.component';

describe('RegisterBagsComponent', () => {
  let component: RegisterBagsComponent;
  let fixture: ComponentFixture<RegisterBagsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RegisterBagsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterBagsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
