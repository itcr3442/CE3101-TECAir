import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RoleLevels } from 'src/app/constants/auth.constants';
import { BookingPackage } from 'src/app/interfaces/booking-package';
import { Flight } from 'src/app/interfaces/flight';
import { AuthService } from 'src/app/services/auth.service';
import { RepositoryService } from 'src/app/services/repository.service';

@Component({
  selector: 'app-flight-search',
  templateUrl: './flight-search.component.html',
  styleUrls: ['./flight-search.component.css']
})
export class FlightSearchComponent implements OnInit {

  seachFlightForm = new FormGroup({
    airport_dep: new FormControl('', [Validators.required]),
    airport_arr: new FormControl('', [Validators.required]),
  })

  bookFlightForm = new FormGroup({
    cardNum: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
    expDate: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
    cve: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
    promoCode: new FormControl(''),
  })

  isBooking: Boolean = false;
  message: string = "";
  message2: string = "";
  flights_list!: Flight[];
  fullRoutes!: string[];
  fullRoute: string = "";
  bookingFlight!: Flight;
  flightNoString!: String;

  constructor(
    private repo: RepositoryService,
    private authRepo: AuthService,
   ) { }

  ngOnInit(): void {}


  onSearch() {
    this.message = ""
    if (this.seachFlightForm.valid) {
      let loginUrl = "search?fromLoc=" + this.seachFlightForm.controls['airport_dep'].value + "&toLoc=" + this.seachFlightForm.controls['airport_arr'].value;
      this.repo.create(loginUrl,"")
      .subscribe((res) => {
        console.log("Result:",res);
        this.flights_list = res.body as Flight[];
        if (this.flights_list.length == 0){
          this.message = "No se encontraron vuelos disponibles para los aeropuertos indicados"
        }else{
          for (let j = 0; j < this.flights_list.length; j++){
            this.fullRoute = "";
            for (let i = 0; i < this.flights_list[j].route.length; i++){
              if (i == 0){
                this.fullRoute = this.flights_list[j].route[i].code;
              }else{
                this.fullRoute = this.fullRoute + " → " + this.flights_list[j].route[i].code;
              }
            }
            this.flights_list[j].flight.fullRoute = this.fullRoute;
          }
        }
      }
    )
    }else{
      this.message = "Por favor ingrese el aeropuerto de salida y el aeropuerto de llegada"
    }
    
  }
  onReserve(flight:Flight){
    this.bookingFlight = flight;
    this.isBooking = true;
    
    console.log("loading resevation view...");
    console.log("Reseved Flight: ", flight);
  }

  onBuy(){
    this.message2 = "";
    if (this.seachFlightForm.valid) {
      console.log("valid booking form...")
      let loginUrl = "flights/" + this.bookingFlight.flight.id + "/book";
      let bookingPackage !: BookingPackage;
      let promoUUID !: any
      if (this.bookFlightForm.controls["promoCode"].value != ""){
        console.log("promo code added...")
        this.repo.getData("promos/search?code=" + this.bookFlightForm.controls["promoCode"].value)
          .subscribe(res => {
          promoUUID = res;
        },
      err => {
        this.message2 = "Código de Promoción Inválida";
      })
        bookingPackage = {pax: this.authRepo.getCredentials().uuid, promo: promoUUID}
      }else{
        console.log("no promo code added...")
        bookingPackage = {pax: this.authRepo.getCredentials().uuid, promo: null};
      }
      console.log("BookingPackage:",bookingPackage)
      this.repo.create(loginUrl,bookingPackage)
      .subscribe((resp: any) => {
        this.message = "Felicitaciones! Se ha reservado su vuelo correctamente";
        this.isBooking = false;
        console.log("going back to flight seach...");
      },
      err => {
        this.message = "FATAL_ERROR";
      })
    }else{
      this.message = "Por favor ingrese correctamente todos los campos solicitados"
    }
  }

  onBack(){
    this.isBooking = false;
    console.log("going back to flight seach...");
  }

}
