import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from './guards/auth.guard';

import { RoleLevels } from 'src/app/constants/auth.constants'

import { LoginComponent } from './components/login/login.component';
import { RegisterUserComponent } from './components/register-user/register-user.component';
import { WorkerAdminComponent } from './components/worker-admin/worker-admin.component';


const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'login/redirect', component: LoginComponent },
  { path: 'register', component: RegisterUserComponent },
  { path: 'worker_register', component: WorkerAdminComponent, canActivate: [AuthGuard], data: { role: RoleLevels.Admin } },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
