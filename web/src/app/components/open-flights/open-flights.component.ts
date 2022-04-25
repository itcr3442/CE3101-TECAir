import { Component, OnInit } from '@angular/core';
import { FlightById } from 'src/app/interfaces/flight-by-id';
import { BoringFlight, FlightsService } from 'src/app/services/flights.service';

@Component({
  selector: 'app-open-flights',
  templateUrl: './open-flights.component.html',
  styleUrls: ['./open-flights.component.css']
})
export class OpenFlightsComponent implements OnInit {

  flightsList: Array<FlightById> = []

  constructor(private flightsService: FlightsService) { }

  ngOnInit(): void {
    this.refreshFlights()
  }

  refreshFlights() {
    this.flightsService.getAllFlights().subscribe((res: any) => {
      console.log(res)
      let allFlights = res.body as BoringFlight[]
      this.flightsList = Array<FlightById>()

      for (let flight of allFlights) {
        this.flightsService.flightById(flight.id).subscribe(res => {
          let flightFromId = res.body as FlightById
          if (flightFromId.flight.state === 0) {
            this.flightsList.push(flightFromId)
          }
        })
      }
    }
    )
  }

  openFlight(id: string) {
    this.flightsService.openFlight(id).subscribe(res => {
      console.log(res);
      this.refreshFlights()
    },
      error => window.alert("No se pudo abrir el vuelo, ocurrió un error: " + error.status))
  }

}
