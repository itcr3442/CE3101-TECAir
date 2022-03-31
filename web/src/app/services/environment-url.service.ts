import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
// Clase que contiene el url del API para las llamadas necesarias
export class EnvironmentUrlService {
  public urlAddress: string = environment.urlAddress;
  constructor() { }
}