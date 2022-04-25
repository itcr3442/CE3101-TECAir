import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FlightById } from 'src/app/interfaces/flight-by-id';
import { User } from 'src/app/interfaces/user';
import { AuthService } from 'src/app/services/auth.service';
import { FlightsService } from 'src/app/services/flights.service';
import { RegisterService } from 'src/app/services/register.service';

@Component({
  selector: 'app-register-bags',
  templateUrl: './register-bags.component.html',
  styleUrls: ['./register-bags.component.css']
})
export class RegisterBagsComponent implements OnInit {

  stage: number = 0
  // 0 = escoger usuario, 1 = escoger vuelo, 2 = escoger asiento
  registerForm = new FormGroup({
    peso: new FormControl('', [Validators.required, Validators.pattern('[0-9.]*')]),
    color: new FormControl('', [Validators.required, Validators.pattern('#[0-9a-fA-F]{6}')]),
  })

  message: string = ""

  // lista de usuarios
  users_list: Array<User> = []
  // id de usuario que se selecciona
  selectedUser = ""
  // vuelos en los que el usuario está checkeado
  userFlights: Array<FlightById> = []

  // vuelo que se escoge para asignar maletas
  flightInfo = {
    uuid: "",
  }

  constructor(private flightsService: FlightsService, private registerService: RegisterService) { }

  title = "seat-chart-generator";

  checkUser(id: string) {
    this.selectedUser = id
    this.stage = 1
    this.loadFlights()
  }

  checkFlight(id: string, no: number) {
    this.flightInfo.uuid = id
    this.stage = 2
  }

  loadFlights() {
    this.flightsService.getUsersCheckedFlights(this.selectedUser).subscribe(res => {
      console.log(res)
      let flight_ids = res.body as Array<string>
      for (let flight_id of flight_ids) {
        this.flightsService.flightById(flight_id).subscribe(res => {
          let flight = res.body as FlightById
          if (flight.flight.state === 1)
            this.userFlights.push(flight)
        })
      }
    })

  }

  ngOnInit(): void {
    this.registerService.getAllUsers().subscribe((res: any) => {
      console.log(res)
      let allUsers = res.body as User[]
      console.log("Users:", allUsers)
      this.users_list = allUsers;
    }
    )
  }

  get id() {
    return this.registerForm.controls['id'].value
  }
  get nvuelo() {
    return this.registerForm.controls['nvuelo'].value
  }
  get peso() {
    return this.registerForm.controls['peso'].value
  }
  get costo() {
    return this.registerForm.controls['costo'].value
  }

  set color(new_color: string) {
    this.registerForm.controls['color'].setValue(new_color)
  }

  get color() {
    return this.registerForm.controls['color'].value
  }


  onSubmit(): void {
    console.log(this.color)

    if (this.registerForm.valid) {
      this.message = ""
      this.registerService.registerBags(this.flightInfo.uuid, this.selectedUser, this.peso, this.color).subscribe(
        (resp: any) => {
          this.registerService.resetForm(this.registerForm)
        },
        err => {
          window.alert("No se pudo registrar la maleta: " + err.status)
        })


    }
    else {
      this.message = "Por favor verifique que ingresó todos los campos en su formato correcto";
    }
  }

}
