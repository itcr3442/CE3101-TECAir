import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service'
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RoleLevels } from 'src/app/constants/auth.constants'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
/**
 * Componente que contiene la página de inicio de sesión
 */
export class LoginComponent implements OnInit {

  loginForm = new FormGroup({
    id: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
    password: new FormControl('', [Validators.required]),
    role: new FormControl(RoleLevels.User, [Validators.required, Validators.min(RoleLevels.User), Validators.max(RoleLevels.Admin)])
  })
  loginMsg: string = ""
  message: string = ""
  logged: boolean;
  constructor(
    private router: Router,
    private authService: AuthService,
  ) {
    this.logged = authService.isLoggedIn()
  }

  refresh(): void {
    if (this.router.url === "/login/redirect") {
      this.router.navigate(['/'])
    }
    else {
      window.location.reload()
    }
  }

  ngOnInit(): void {
    this.logged = this.authService.isLoggedIn()
    if (this.router.url === "/login/redirect") {
      this.message = "Debe ingresar al sistema para poder acceder a esa página"
    }
  }

  // Este get es para poder usarlo dentro de template
  public get RoleLevels(): typeof RoleLevels {
    return RoleLevels;
  }

  get role() {
    return this.loginForm.controls['role'].value
  }

  get id() {
    return this.loginForm.controls['id'].value
  }

  get password() {
    return this.loginForm.controls['password'].value
  }
  /**
   * Función que se llama para salir de la sesión.
   * Esta es llamada al apretar el botón correspondiente.
   */
  logout() {
    this.authService.logout()
    this.logged = false
    this.refresh()
  }
  /**
  * Método que se llama para verificar con el servido si los datos introducidos
  * son válidos para el inicio de sesión.
  */
  onSubmit() {
    if (this.loginForm.valid) {

      this.authService.login(this.id, this.password, this.role).subscribe(
        (res: boolean) => {
          if (res) {
            this.logged = true
            this.loginMsg = ""
            this.refresh()
          }
          else {
            this.loginMsg = "Cédula o contraseña incorrectos";
          }
        }
      )
    }
    else {
      this.loginMsg = "Por favor verifique que ingresó todos los campos correctamente y su cédula solo contiene dígitos";
    }
  }
}
