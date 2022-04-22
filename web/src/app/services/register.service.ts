import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { RepositoryService } from './repository.service';
import { map, of } from 'rxjs';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class RegisterService {

  constructor(private repositoryService: RepositoryService, private authService: AuthService) { }

  // TODO: cambiar los parámetros al modelo de trabajador cuando soto termine la base 
  public register_worker(id: string, password: string, name: string, apellido1: string, apellido2: string) {

    let token = this.authService.getCredentials()

    let registerUrl = "trabajadores?cedula=" + token.id + "&password_hash=" + token.password

    let hash = password
    let new_worker = {
      "cedula": id,
      "password_hash": hash,
      "nombre": name,
      "primer_apellido": apellido1,
      "segundo_apellido": apellido2,
    }

    console.log("New worker: " + JSON.stringify(new_worker))


    console.log("POST url: " + registerUrl)
    //TODO : también arreglar esto cuando haya DB
    return of(1)
    // return this.repositoryService.create(
    //   registerUrl, new_worker)
    //   .pipe(map((res: any) => {
    //     console.log("post result: " + JSON.stringify(res))
    //     return res.success
    //   }
    //   ))
  }

  /**
 * Método que realiza el request al servidor para obtener todos
 * los trabajadores para mostrarlos en la lista correspondiente.
 */
  public getAllWorkers = () => {
    let token = this.authService.getCredentials()
    let registerUrl = "trabajadores?cedula_admin=" + token.id + "&password_hash=" + token.password
    console.log(registerUrl);

    // TODO: cambiar esto a la llamada cuando esté la DB
    return of([])
    // return this.repositoryService.getData(registerUrl)
  }

  public resetForm = (formGroup: FormGroup) => {
    Object.values(formGroup.controls).forEach((control) => control.reset())
  }
}
