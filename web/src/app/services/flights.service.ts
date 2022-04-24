import { Injectable } from '@angular/core';
import { RepositoryService } from './repository.service';

@Injectable({
  providedIn: 'root'
})
export class FlightsService {

  constructor(private repo: RepositoryService) { }

  public flightById(id: string) {
    return this.repo.getData("flights/" + id)
  }
}
