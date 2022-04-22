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
    id: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
    nombre: new FormControl('', [Validators.required]),
    apellido1: new FormControl('', [Validators.required]),
    apellido2: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required]),
    phone: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')])
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

  get id() {
    return this.registerForm.controls['id'].value
  }

  get password() {
    return this.registerForm.controls['password'].value
  }

  get name() {
    return this.registerForm.controls['nombre'].value
  }

  get apellido1() {
    return this.registerForm.controls['apellido1'].value
  }
  get apellido2() {
    return this.registerForm.controls['apellido2'].value
  }

  /**
   * Método que se ejecuta al apretar el botón registrar.
   * Verifica si el ususario ha iniciado sesión, para obtener los valores de las
   * casillas correspondientes y así registrar el nuevo trabajador en la base de datos del servidor
   */
  onSubmit() {
    if (this.registerForm.valid) {

      if (!this.authService.isLoggedIn()) {
        this.router.navigate(['/login/redirect']);
        return
      }

      this.registerService.register_worker(this.id, this.password, this.name, this.apellido1, this.apellido2).subscribe((success: number) => {
        if (success === 1) {
          console.log("Register successful");
          this.message = ""
          this.registerService.resetForm(this.registerForm)
        }
        else if (success === -1) {
          this.message = "La cédula dada ya se encuentra registrada en el sistema.";
        } else if (success === -2) {
          this.message = "El rol seleccionado no es válido.";
        } else if (success === -3) {
          this.message = "Usted no cuenta con permisos suficientes para realizar esta acción.";
        }
      })



    }
    else {
      this.message = "Por favor verifique que ingresó todos los campos, la cédula solo contiene dígitos y escogió un rol";
    }
  }

}
