import { Injectable } from '@angular/core';
import { RepositoryService } from './repository.service';

@Injectable({
  providedIn: 'root'
})
export class PromosService {

  constructor(private repo: RepositoryService) { }

  /**
   * 
   * @returns retorna los datos de las promociones disponibles
   */
  public getPromos() {
    return this.repo.getData("promos")
  }
}
