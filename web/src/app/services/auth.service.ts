import { Injectable, ɵisObservable } from '@angular/core';
import { RepositoryService } from './repository.service';
import { map, Observable, of, Subscription } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private repo: RepositoryService) { }

  /**
   * Remueve los datos de local storage que mantienen la sesión del usuario
   */
  public logout(): void {
    localStorage.setItem('isLoggedIn', 'false');
    localStorage.removeItem('token');
  }
  /**
   * 
   * @returns Revisa que los datos necesarios estén presentes en local storage para decir que el usuario está ingresado
   */
  public isLoggedIn(): boolean {
    if (localStorage.getItem('isLoggedIn') == "true") {
      let token = JSON.parse(localStorage.getItem('token') || '{}')
      if (token.hasOwnProperty('id') && token.hasOwnProperty('password')) {
        return true;
      }
    }
    return false;
  }
  /**
   * Obtiene la info del usuario actual de local storage, llamar después de isLoggedIn()
   * @returns Object con 'id' y 'password' fields
   */
  public getCredentials(): any {
    return JSON.parse(localStorage.getItem('token') || '{}')
  }

  public login(id: string, password: string, role: number) {

    let hash = password
    let loginUrl = "check_login?cedula=" + id.trim() + "&password_hash=" + hash

    // TODO: descomentar esto cuando esté el servidor 
    // return this.repo.getData(
    //   loginUrl)
    return of({ success: true }) // y comentar esto
      .pipe(map((res: any) => {
        if (res.success) {
          console.log("Login successful");
          localStorage.setItem('isLoggedIn', "true");
          localStorage.setItem('token', JSON.stringify({ "id": id, "password": hash, role }));
          // this.logged = true
          // this.message = ""
          return true
        }
        return false
      }
      ))
  }
}