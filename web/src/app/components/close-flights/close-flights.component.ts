import { Component, OnInit } from '@angular/core';
import jsPDF from 'jspdf';
import { FlightById } from 'src/app/interfaces/flight-by-id';
import { User } from 'src/app/interfaces/user';
import { BoringFlight, FlightsService } from 'src/app/services/flights.service';
import { RegisterService } from 'src/app/services/register.service';


// pasagero donde "pax" es el id
interface Passenger {
  pax: string,
  bags: number
}

// pasagero donde "pax" es un usuario completo
interface PassengerUser {
  pax: User,
  bags: number
}

@Component({
  selector: 'app-close-flights',
  templateUrl: './close-flights.component.html',
  styleUrls: ['./close-flights.component.css']
})
export class CloseFlightsComponent implements OnInit {
  flightsList: Array<FlightById> = []
  selectedFlight: FlightById | null = null
  passengers: Array<PassengerUser> = []
  totalBags: number = 0

  constructor(private flightsService: FlightsService, private registerService: RegisterService) { }

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
          if (flightFromId.flight.state === 1) {
            this.flightsList.push(flightFromId)
          }
        })
      }
    }
    )
  }

  sleep(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  generateReport() {
    this.sleep(1000).then(() => {

      const doc = new jsPDF();
      doc.html(document.getElementById('report')!, {
        callback: function (doc) {
          window.open(window.URL.createObjectURL(doc.output('blob')));
        },
        html2canvas: {
          scale: doc.internal.pageSize.getWidth() / window.innerWidth,
          useCORS: true,
          windowWidth: 1000,
        },
      });
    })
  }

  closeFlight(id: string) {
    this.passengers = []
    this.totalBags = 0

    this.flightsService.closeFlight(id).subscribe(res => {
      console.log(res);
      this.refreshFlights()
      let passengers_b = res.body as Passenger[]
      for (let passenger of passengers_b) {
        this.totalBags += passenger.bags
        this.registerService.getUser(passenger.pax).subscribe(res => this.passengers.push({ pax: res.body as User, bags: passenger.bags }))
      }
      this.generateReport()
    },
      error => window.alert("No se pudo cerrar el vuelo, ocurrió un error: " + error.status))
  }

}
