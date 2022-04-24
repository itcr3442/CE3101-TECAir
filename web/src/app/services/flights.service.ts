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

  public checkInSegment(id: string, userId: string, seat: number) {
    let checkinInfo = {
      pax: userId,
      seat
    }

    return this.repo.create("segments/" + id + "/checkin", checkinInfo)
  }

  public getUsersBookedFlights(id: string) {
    return this.repo.getData("users/" + id + "/open")
  }
}
