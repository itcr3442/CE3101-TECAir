import { Component, Input, OnInit } from '@angular/core';
import { FlightById } from 'src/app/interfaces/flight-by-id';
import { Promo } from 'src/app/interfaces/promo';
import { FlightsService } from 'src/app/services/flights.service';
import { PromosService } from 'src/app/services/promos.service';

@Component({
  selector: 'app-promos',
  templateUrl: './promos.component.html',
  styleUrls: ['./promos.component.css']
})
export class PromosComponent implements OnInit {

  @Input() fontSize: number = 1;

  num_promos: number
  promos: Promo[] = []
  flight_descriptors: string[] = ["", "", "", "", ""]

  constructor(private promoService: PromosService, private flightsService: FlightsService) {
    this.num_promos = 0
    this.promoService.getPromos().subscribe((res) => {
      console.log(res)
      this.promos = res.body as Promo[]
      console.log("Promos:", this.promos)
      this.num_promos = this.promos.length
      for (const [i, promo] of this.promos.entries()) {
        flightsService.flightById(promo.flight).subscribe(res => {
          console.log(res)
          let flight = res.body as FlightById
          this.flight_descriptors[i] = flight.route[0].code + " -> " + flight.route[flight.route.length - 1].code
        })
      }
    })
  }


  ngOnInit(): void {
  }

}
