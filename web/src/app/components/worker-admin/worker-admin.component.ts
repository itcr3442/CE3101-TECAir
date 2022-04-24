import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { RegisterService } from 'src/app/services/register.service';
import { Worker } from 'src/app/interfaces/worker';

@Component({
  selector: 'app-worker-admin',
  templateUrl: './worker-admin.component.html',
  styleUrls: ['./worker-admin.component.css']
})
export class WorkerAdminComponent implements OnInit {

  public worker_list!: Worker[];

  registerForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    firstName: new FormControl('', [Validators.required]),
    lastName: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required]),
    phone: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
  })
  message: string = ""

  constructor(
    private router: Router,
    private authService: AuthService,
    private registerService: RegisterService,
  ) {
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login/redirect']);
      return
    }
    this.registerService.getAllWorkers().subscribe((res: any) => {
      console.log("Get all workers res:" + JSON.stringify(res));
      this.worker_list = res as Worker[];
    }
    )
  }

  get username() {
    return this.registerForm.controls['username'].value
  }

  get password() {
    return this.registerForm.controls['password'].value
  }

  get firstName() {
    return this.registerForm.controls['firstName'].value
  }
  get lastName() {
    return this.registerForm.controls['lastName'].value
  }
  get email() {
    return this.registerForm.controls['email'].value
  }
  get phone() {
    return this.registerForm.controls['phone'].value
  }

  /**
   * Método que se ejecuta al apretar el botón registrar.
   * Verifica si el ususario ha iniciado sesión, para obtener los valores de las
   * casillas correspondientes y así registrar el nuevo trabajador en la base de datos del servidor
   */
  onSubmit() {
    if (this.registerForm.valid) {
      this.message = ""
      this.registerService.register_worker(this.username, this.password, this.firstName, this.lastName, this.phone, this.email).subscribe(
        (resp: any) => {
          this.message = "Funcionario registrado correctamente";
          this.registerService.resetForm(this.registerForm)
        },
        err => {
          if (err.status == 409) {
            this.message = "Nombre de usuario ya está tomado";
          }
        })

    }
    else {
      this.message = "Por favor verifique que ingresó todos los campos correctamente y su # de tel solo contiene dígitos";
    }
  }

}
