import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FlightById } from 'src/app/interfaces/flight-by-id';
import { BoringFlight, FlightsService, Segment } from 'src/app/services/flights.service';
import { RegisterService } from 'src/app/services/register.service';

interface Airport {
  id: string,
  code: string,
  comment: string,
}
interface Aircraft {
  id: string,
  code: string,
  seats: number,
}

@Component({
  selector: 'app-register-flight',
  templateUrl: './register-flight.component.html',
  styleUrls: ['./register-flight.component.css']
})
export class RegisterFlightComponent implements OnInit {

  myDatePickerFrom: string = ""
  myDatePickerTo: string = ""


  flightsList: Array<FlightById> = []
  airportsList: Array<Airport> = []
  aircraftsList: Array<Aircraft> = []

  totalAirports: number = 2

  registerForm = new FormGroup({
    no: new FormControl('', [Validators.required]),
    precio: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
    comment: new FormControl('', [Validators.required])
  })
  message: string = ""

  constructor(private flightsService: FlightsService, private registerService: RegisterService) { }

  ngOnInit(): void {
    this.refreshFlights()
    this.flightsService.getAircraft().subscribe(res => {
      console.log("Aircraft:", res)
      this.aircraftsList = res.body as Aircraft[]
    })
    this.flightsService.getAirports().subscribe(res => {
      console.log("Airports:", res)
      this.airportsList = res.body as Airport[]
    })
  }

  addAirport() {
    this.totalAirports += 1
  }
  decreaseAirport() {
    this.totalAirports -= 1
    if (this.totalAirports < 2) { this.totalAirports = 2 }
  }
  deleteFlight(id: string) {
    const userResponse = window.confirm('Seguro que desea eliminar el vuelo?')
    if (userResponse) {
      this.flightsService.delete_flight(id).subscribe(res => {
        console.log("Vuelo eliminado eliminado:", res)
        this.refreshFlights()
      })
    }
  }

  clearForm() {
    this.registerService.resetForm(this.registerForm)
    this.totalAirports = 2
    let airportSelect: any = document.getElementById('airport-OG');
    airportSelect.value = this.airportsList[0].code

    let planeSelect: any = document.getElementById('plane-OG');
    planeSelect.value = this.aircraftsList[0].code

    let departureTimeEle: any = document.getElementById("partida-OG")
    departureTimeEle.value = ""

    let arrivalTimeEle: any = document.getElementById("llegada-OG")
    arrivalTimeEle.value = ""
  }

  refreshFlights() {
    this.flightsService.getAllFlights().subscribe((res: any) => {
      console.log(res)
      let allFlights = res.body as BoringFlight[]
      this.flightsList = Array<FlightById>()

      for (let flight of allFlights) {
        this.flightsService.flightById(flight.id).subscribe(res => {
          let flightFromId = res.body as FlightById
          this.flightsList.push(flightFromId)
        })
      }

    }
    )
  }

  counter() {
    return new Array(this.totalAirports - 1);
  }

  checkValidity() {
    let airportSelect: any = document.getElementById('airport-OG');
    let airport: string = airportSelect.options[airportSelect.selectedIndex].value;

    let planeSelect: any = document.getElementById('plane-OG');
    let plane: string = planeSelect.options[planeSelect.selectedIndex].value;

    let departureTimeEle: any = document.getElementById("partida-OG")
    let departureTimeStr: string = departureTimeEle.value
    let departureTime: Date = new Date(departureTimeStr)
    if (departureTime.getTime()) {
      console.log("departure time '" + departureTime.toISOString(), "' is valid")
    } else {
      return false
    }

    let arrivalTimeEle: any = document.getElementById("llegada-OG")
    let arrivalTimeStr: string = arrivalTimeEle.value
    let arrivalTime: Date = new Date(arrivalTimeStr)

    if (arrivalTime.getTime()) {
      console.log("departure time '" + arrivalTime.toISOString(), "' is valid")
    } else {
      return false
    }

    if (arrivalTimeStr.length < 1 || departureTimeStr.length < 1) {
      return false
    }

    if (airport.length < 1 || plane.length < 1) {
      return false
    }

    for (let i = 0; i < this.totalAirports - 1; i++) {
      let airportSelect: any = document.getElementById('select-airport-' + i);
      let airport: string = airportSelect.options[airportSelect.selectedIndex].value;

      console.log("i:" + i + "\n\tairport:" + airport)

      if (airport.length < 1) {
        return false
      }
    }

    console.log("Checkpoint!")

    for (let i = 0; i < this.totalAirports - 2; i++) {
      console.log("Checkpoint!", i)
      let departureTimeEle: any = document.getElementById("partida-" + i)
      let departureTimeStr: string = departureTimeEle.value
      let departureTime: Date = new Date(departureTimeStr)
      if (departureTime.getTime()) {
        console.log("departure time '" + departureTime.toISOString(), "' is valid")
      } else {
        return false
      }

      let arrivalTimeEle: any = document.getElementById("llegada-" + i)
      let arrivalTimeStr: string = arrivalTimeEle.value
      let arrivalTime: Date = new Date(arrivalTimeStr)

      if (arrivalTime.getTime()) {
        console.log("departure time '" + arrivalTime.toISOString(), "' is valid")
      } else {
        return false
      }

      let planeSelect: any = document.getElementById('select-plane-' + i);
      let plane: string = planeSelect.options[planeSelect.selectedIndex].value;

      if (arrivalTimeStr.length < 1 || departureTimeStr.length < 1 || plane.length < 1) {
        return false
      }
    }

    console.log("All good! :D")

    return true
  }
  get no() {
    return this.registerForm.controls['no'].value
  }
  get precio() {
    return this.registerForm.controls['precio'].value
  }
  get comment() {
    return this.registerForm.controls['comment'].value
  }

  get airports(): Array<string> {

    let airports: Array<string> = []

    let airportSelect: any = document.getElementById('airport-OG');
    let airport: string = airportSelect.options[airportSelect.selectedIndex].value;

    airports.push(airport)

    for (let i = 0; i < this.totalAirports - 1; i++) {
      let airportSelect: any = document.getElementById('select-airport-' + i);
      let airport: string = airportSelect.options[airportSelect.selectedIndex].value;
      airports.push(airport)
    }
    return airports
  }

  get segments(): Array<Segment> {
    let segments: Array<Segment> = []

    let planeSelect: any = document.getElementById('plane-OG');
    let plane: string = planeSelect.options[planeSelect.selectedIndex].value;

    let departureTimeEle: any = document.getElementById("partida-OG")
    let departureTimeStr: string = departureTimeEle.value
    let departureTime: Date = new Date(departureTimeStr)

    let arrivalTimeEle: any = document.getElementById("llegada-OG")
    let arrivalTimeStr: string = arrivalTimeEle.value
    let arrivalTime: Date = new Date(arrivalTimeStr)

    let segmentOG: Segment = {
      aircraft: plane,
      fromTime: departureTime.toISOString(),
      toTime: arrivalTime.toISOString()
    }

    segments.push(segmentOG)

    for (let i = 0; i < this.totalAirports - 2; i++) {

      let planeSelect: any = document.getElementById('select-plane-' + i);
      let plane: string = planeSelect.options[planeSelect.selectedIndex].value;

      let departureTimeEle: any = document.getElementById("partida-" + i)
      let departureTimeStr: string = departureTimeEle.value
      let departureTime: Date = new Date(departureTimeStr)

      let arrivalTimeEle: any = document.getElementById("llegada-" + i)
      let arrivalTimeStr: string = arrivalTimeEle.value
      let arrivalTime: Date = new Date(arrivalTimeStr)

      let segment: Segment = {
        aircraft: plane,
        fromTime: departureTime.toISOString(),
        toTime: arrivalTime.toISOString()
      }

      segments.push(segment)

    }

    return segments
  }

  onSubmit() {
    console.log("Custom:", this.checkValidity())
    console.log("form:", this.registerForm.valid)
    if (this.checkValidity() && this.registerForm.valid) {
      this.message = ""
      this.flightsService.registerFlight(this.no, this.precio, this.comment, this.segments, this.airports).subscribe(res => {
        console.log("Vuelo creado:", res);
        this.refreshFlights()
        this.clearForm()
      },
        (error: any) => {
          window.alert("No se pudo registrar el vuelo: " + error.status)
        })
    }
    else {
      this.message = "Por favor verifique que ingresó todos los campos correctamente y el costo del viaje solo contiene dígitos, también que las fechas fueron ingresadas en un formato válido.";
    }
  }
}
