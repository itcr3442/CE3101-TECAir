import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import {
  NgxMatDatetimePickerModule,
  NgxMatNativeDateModule,
  NgxMatTimepickerModule
} from '@angular-material-components/datetime-picker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ColorPickerModule } from 'ngx-color-picker';


import { AuthGuard } from './guards/auth.guard';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { RegisterUserComponent } from './components/register-user/register-user.component';
import { WorkerAdminComponent } from './components/worker-admin/worker-admin.component';
import { PromosComponent } from './components/promos/promos.component';
import { PromosPageComponent } from './components/promos-page/promos-page.component';
import { FlightSearchComponent } from './components/flight-search/flight-search.component';
import { CheckInComponent } from './components/check-in/check-in.component';
import { UserAdminComponent } from './components/user-admin/user-admin.component';
import { RegisterFlightComponent } from './components/register-flight/register-flight.component';
import { OpenFlightsComponent } from './components/open-flights/open-flights.component';
import { CloseFlightsComponent } from './components/close-flights/close-flights.component';
import { RegisterBagsComponent } from './components/register-bags/register-bags.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavbarComponent,
    RegisterUserComponent,
    WorkerAdminComponent,
    PromosComponent,
    PromosPageComponent,
    FlightSearchComponent,
    CheckInComponent,
    UserAdminComponent,
    RegisterFlightComponent,
    OpenFlightsComponent,
    CloseFlightsComponent,
    RegisterBagsComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    TooltipModule,
    MatDatepickerModule,
    MatInputModule,
    NgxMatDatetimePickerModule,
    NgxMatTimepickerModule,
    NgxMatNativeDateModule,
    BrowserAnimationsModule,
    ColorPickerModule
  ],

  providers: [AuthGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }
