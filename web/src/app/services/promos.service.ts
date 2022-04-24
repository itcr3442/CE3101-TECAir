import { Injectable } from '@angular/core';
import { RepositoryService } from './repository.service';

@Injectable({
  providedIn: 'root'
})
export class PromosService {

  constructor(private repo: RepositoryService) { }

  public getPromos() {
    return this.repo.getData("promos")
  }
}
