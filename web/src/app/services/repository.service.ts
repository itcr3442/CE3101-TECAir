import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentUrlService } from './environment-url.service';

// Service for handling server requests

@Injectable({
  providedIn: 'root'
})
export class RepositoryService {

  constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

  /**
   * GET request
   * @param route endpoint relativo
   * @returns Observable con datos retornados por el server
   */
  public getData = (route: string) => {
    return this.http.get(this.createCompleteRoute(route, this.envUrl.urlAddress), { observe: 'response' });
  }

  /**
   * POST request
   * @param route endpoint relativo
   * @param body contenidos JSON requeridos por endpoint
   * @returns Observable con datos retornados por el server
   */
  public create = (route: string, body: any) => {
    return this.http.post(this.createCompleteRoute(route, this.envUrl.urlAddress), body, { headers: this.generateHeaders().headers, observe: 'response' });
  }

  // Junta el url base del API con la ruta relative de los
  private createCompleteRoute = (route: string, envAddress: string) => {
    return `${envAddress}/${route}`;
  }

  private generateHeaders = () => {
    return {
      headers: new HttpHeaders({
        "Access-Control-Allow-Origin": "*", // este header es para permitir todos los CORS necesarios de los requests
        'Content-Type': 'application/json'
      })
    }
  }
}