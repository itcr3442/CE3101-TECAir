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

  /**
   * 
   * @param id id del vuelo
   * @returns información detallada del vuelo
   */
  public flightById(id: string) {
    return this.repo.getData("flights/" + id)
  }

  /**
   * Realiza el check in de un usuario para un segmento de su vuelo
   * @param id id del segmento
   * @param userId id del usuario
   * @param seat el num de asiento del usuario
   * @returns info de la respuesta
   */
  public checkInSegment(id: string, userId: string, seat: number) {
    let checkinInfo = {
      pax: userId,
      seat
    }

    return this.repo.create("segments/" + id + "/checkin", checkinInfo)
  }

  /**
   * 
   * @param id id del usuario
   * @returns Retorna los vuelos reservados del usuario
   */
  public getUsersBookedFlights(id: string) {
    return this.repo.getData("users/" + id + "/open")
  }

  public getUsersCheckedFlights(id: string) {
    return this.repo.getData("users/" + id + "/checked")
  }

  public getAirports() {
    return this.repo.getData("airports")
  }
  public getAircraft() {
    return this.repo.getData("aircraft")
  }

  /**
   * Para registrar un vuelo
   * @param no num de vuelo
   * @param price precio de tiquete
   * @param comment detalle adicional
   * @param segments segmentos del vuelo ed tipo Segment
   * @param airports aeropuertos (n+1 segmentos)
   * @returns información de la respuesta
   */
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

  /**
   * Borra un vuelo
   * @param id 
   * @returns 
   */
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
