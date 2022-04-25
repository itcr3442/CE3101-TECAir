import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from './guards/auth.guard';

import { RoleLevels } from 'src/app/constants/auth.constants'

import { LoginComponent } from './components/login/login.component';
import { RegisterUserComponent } from './components/register-user/register-user.component';
import { WorkerAdminComponent } from './components/worker-admin/worker-admin.component';
import { PromosPageComponent } from './components/promos-page/promos-page.component';
import { FlightSearchComponent } from './components/flight-search/flight-search.component';
import { CheckInComponent } from './components/check-in/check-in.component';
import { UserAdminComponent } from './components/user-admin/user-admin.component';
import { RegisterFlightComponent } from './components/register-flight/register-flight.component';
import { OpenFlightsComponent } from './components/open-flights/open-flights.component';
import { CloseFlightsComponent } from './components/close-flights/close-flights.component';
import { RegisterBagsComponent } from './components/register-bags/register-bags.component';


const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'login/redirect', component: LoginComponent },
  { path: 'register', component: RegisterUserComponent },
  { path: 'worker_register', component: WorkerAdminComponent, canActivate: [AuthGuard], data: { role: RoleLevels.Admin } },
  { path: 'user_register', component: UserAdminComponent, canActivate: [AuthGuard], data: { role: RoleLevels.Admin } },
  { path: 'promos_page', component: PromosPageComponent },
  { path: 'flight_search', component: FlightSearchComponent, canActivate: [AuthGuard], data: { role: RoleLevels.User } },
  { path: 'check-in', component: CheckInComponent, canActivate: [AuthGuard], data: { role: RoleLevels.Admin } },
  { path: 'flight_register', component: RegisterFlightComponent, canActivate: [AuthGuard], data: { role: RoleLevels.Admin } },
  { path: 'flight_open', component: OpenFlightsComponent, canActivate: [AuthGuard], data: { role: RoleLevels.Admin } },
  { path: 'flight_close', component: CloseFlightsComponent, canActivate: [AuthGuard], data: { role: RoleLevels.Admin } },
  { path: 'bags_register', component: RegisterBagsComponent, canActivate: [AuthGuard], data: { role: RoleLevels.Admin } },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
