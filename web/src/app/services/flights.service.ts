import { Injectable } from '@angular/core';
import { RepositoryService } from './repository.service';

export interface BoringFlight {
  id: string,
  no: number,
  comment: string,
  price: number,
  state: number
}


export interface Segment {
  aircraft: string,
  fromTime: string,
  toTime: string
}

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

  public getAirports() {
    return this.repo.getData("airports")
  }
  public getAircraft() {
    return this.repo.getData("aircraft")
  }

  public registerFlight(no: number, price: number, comment: string, segments: Segment[], airports: string[]) {
    let flight = {
      no,
      price,
      comment,
      segments,
      airports
    }
    return this.repo.create("flights", flight)
  }

  public delete_flight(id: string) {
    return this.repo.delete(
      "flights/" + id)
  }

  public getAllFlights() {
    return this.repo.getData("flights")
  }

  public openFlight(id: string) {
    return this.repo.create("flights/" + id + "/open", {})
  }

  public closeFlight(id: string) {
    return this.repo.create("flights/" + id + "/close", {})
  }
}
