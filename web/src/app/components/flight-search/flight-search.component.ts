import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RoleLevels } from 'src/app/constants/auth.constants';
import { Flights } from 'src/app/interfaces/flights';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-flight-search',
  templateUrl: './flight-search.component.html',
  styleUrls: ['./flight-search.component.css']
})
export class FlightSearchComponent implements OnInit {

  message: string = ""

  public flights_list!: Flights[];

  constructor() { }

  ngOnInit(): void {
    this.getAllFlights();
  }

  public getAllFlights = () =>{
    /** "Implementar bien cuando esté la base de datos"
    let loginUrl = "vuelos";
    this.repo.getData(loginUrl)
    .subscribe(res => {
        console.log("Result:" + JSON.stringify(res));
        this.flights_list = res as Flights[];
      }
    )
     */
    
  }

  onSearch() {
    this.message = "Cuando Alejandro termine la base de datos >:("
  }

}
