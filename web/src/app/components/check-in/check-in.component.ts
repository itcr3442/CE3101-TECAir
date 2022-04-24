import { Component, OnInit } from '@angular/core';
import { PlacementForBs5 } from 'ngx-bootstrap/positioning';
import { FlightById } from 'src/app/interfaces/flight-by-id';
import { FlightsService } from 'src/app/services/flights.service';
import jsPDF from 'jspdf';
import { AuthService } from 'src/app/services/auth.service';
import { RegisterService } from 'src/app/services/register.service';
import { User } from 'src/app/interfaces/user';


interface Cart {
  selectedSeat: string | null,
  cartId: string,
  eventId: number
}

@Component({
  selector: 'app-check-in',
  templateUrl: './check-in.component.html',
  styleUrls: ['./check-in.component.css']
})
export class CheckInComponent implements OnInit {
  stage: number = 0
  // 0 = escoger usuario, 1 = escoger vuelo, 2 = escoger asiento
  seatConfig: any = null;
  seatmap = Array<any>();
  seatChartConfig = {
    showRowsLabel: false,
    showRowWisePricing: false,
    newSeatNoForRow: false
  };
  cart: Cart = {
    selectedSeat: null,
    cartId: "",
    eventId: 0
  };

  users_list: Array<User> = []
  selectedUser = ""
  userFlights: Array<FlightById> = []

  flightInfo = {
    uuid: "",
    segment: 0
  }

  constructor(private flightsService: FlightsService, private authService: AuthService, private registerService: RegisterService) { }

  title = "seat-chart-generator";

  checkUser(id: string) {
    this.selectedUser = id
    this.stage = 1
    this.loadFlights()
  }

  checkFlight(id: string, no: number) {
    this.flightInfo.uuid = id
    let segmentSelect: any = document.getElementById('segSelect-' + no);
    this.flightInfo.segment = +segmentSelect.options[segmentSelect.selectedIndex].value;
    console.log("Segment:", this.flightInfo)
    this.loadSeats()
    this.stage = 2

  }

  loadFlights() {
    this.flightsService.getUsersBookedFlights(this.selectedUser).subscribe(res => {
      console.log(res)
      let flight_ids = res.body as Array<string>
      for (let flight_id of flight_ids) {
        this.flightsService.flightById(flight_id).subscribe(res => {
          let flight = res.body as FlightById
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

  public loadSeats() {
    const seatsPerRow = 6

    this.flightsService.flightById(this.flightInfo.uuid).subscribe(res => {
      console.log(res)
      let flight = res.body as FlightById
      console.log("Flight:", flight)
      let total_seats = flight.segments[this.flightInfo.segment].seats
      let rows = Math.floor(total_seats / seatsPerRow)
      let remainderSeats = total_seats % seatsPerRow

      let seat_map: Array<any> = [];
      for (let i = 0; i < rows; i++) {
        seat_map.push({
          seat_label: "" + i,
          layout: "ggg_ggg"
        })
      }
      console.log(seat_map)

      if (remainderSeats > 0) {
        console.log(seat_map)

        let gs = "g".repeat(remainderSeats)
        let remainderSeatsLayout = gs.slice(0, seatsPerRow / 2) + "_".repeat(7 - remainderSeats) + gs.slice(seatsPerRow / 2)
        seat_map.push({
          seat_label: "" + rows,
          layout: remainderSeatsLayout
        })
      }
      console.log(seat_map)

      //Process a simple bus layout
      this.seatConfig = [
        {
          seat_map
        }
      ];
      this.processSeatChart(this.seatConfig);

      let unavailString = ""

      for (let takenSeat of flight.segments[this.flightInfo.segment].unavail) {
        let takenRow = Math.floor(takenSeat / seatsPerRow)
        let takenCol = takenSeat % seatsPerRow
        unavailString += takenRow + "_" + takenCol + ","
      }

      this.blockSeats(unavailString);
    })
  }
  processBooking() {
    this.registerService.getUser(this.selectedUser).subscribe(res => {
      let userInfo = res.body as User
      this.flightsService.flightById(this.flightInfo.uuid).subscribe(res => {
        let flight = res.body as FlightById
        let flightInfo = flight.flight
        let segmentInfo = flight.segments[this.flightInfo.segment]
        let seat = this.cart.selectedSeat != null ? +this.cart.selectedSeat.split(' ')[1] : 0
        this.flightsService.checkInSegment(segmentInfo.id, this.selectedUser, seat)
          .subscribe(res => {
            const pdf = new jsPDF();
            pdf.setFontSize(11)
            pdf.text("Pasajero: " + userInfo.firstName + " " + userInfo.lastName, 5, 10);
            // pdf.text("Fecha de emisión: " + day + "/" + month + "/" + year, 5, 15);

            pdf.setFontSize(8)
            pdf.text("Fecha y hora de partida: " + (new Date(segmentInfo.fromTime)).toString(), 5, 20);
            pdf.text("Fecha y hora de llegada: " + (new Date(segmentInfo.fromTime)).toString(), 5, 23.5);

            const letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            let gateLetter = letters.charAt(Math.floor(Math.random() * letters.length));
            let gateNumber = Math.floor(Math.random() * letters.length)

            pdf.setFontSize(10)
            pdf.text("Puerta de abordaje: " + gateLetter + gateNumber, 5, 60)
            pdf.text("No. Vuelo: " + flightInfo.no, 5, 65)
            pdf.text("Asiento: " + seat, 5, 70)

            pdf.save("Boarding.pdf");

          }, error => {
            window.alert("No se pudo realizar el check-in, ocurrió un error en el servidor: " + error.status)
          })

      })
    })

  }

  public processSeatChart(map_data: any[]) {
    if (map_data.length > 0) {
      var seatNoCounter = 0;
      for (let __counter = 0; __counter < map_data.length; __counter++) {
        var row_label = "";
        var item_map = map_data[__counter].seat_map;

        //Get the label name and price
        row_label = "Row " + item_map[0].seat_label + " - ";
        if (item_map[item_map.length - 1].seat_label != " ") {
          row_label += item_map[item_map.length - 1].seat_label;
        } else {
          row_label += item_map[item_map.length - 2].seat_label;
        }

        item_map.forEach((map_element: any) => {
          var mapObj = {
            seatRowLabel: map_element.seat_label,
            seats: Array<any>(),
            seatPricingInformation: row_label
          };
          row_label = "";
          var seatValArr = map_element.layout.split("");
          if (this.seatChartConfig.newSeatNoForRow) {
            seatNoCounter = 1; //Reset the seat label counter for new row
          }
          var totalItemCounter = 1;
          seatValArr.forEach((item: any) => {
            var seatObj: any = {
              key: map_element.seat_label + "_" + totalItemCounter,
              status: "available"
            };

            if (item != "_") {
              seatObj["seatLabel"] =
                map_element.seat_label + " " + seatNoCounter;
              if (seatNoCounter < 10) {
                seatObj["seatNo"] = "0" + seatNoCounter;
              } else {
                seatObj["seatNo"] = "" + seatNoCounter;
              }

              seatNoCounter++;
            } else {
              seatObj["seatLabel"] = "";
            }
            totalItemCounter++;
            mapObj.seats.push(seatObj);
          });
          console.log(" \n\n\n Seat Objects ", mapObj);
          this.seatmap.push(mapObj);
        });
      }
    }
  }

  public selectSeat(seatObject: any) {
    console.log("Seat to block: ", seatObject);
    if (seatObject.seatLabel != this.cart.selectedSeat) {
      seatObject.status = "booked";
      this.cart.selectedSeat = seatObject.seatLabel;
    } else {
      seatObject.status = "available";
      this.cart.selectedSeat = null
    }
  }

  public blockSeats(seatsToBlock: string) {
    if (seatsToBlock != "") {
      var seatsToBlockArr = seatsToBlock.split(",");
      for (let index = 0; index < seatsToBlockArr.length; index++) {
        var seat = seatsToBlockArr[index] + "";
        var seatSplitArr = seat.split("_");
        console.log("Split seat: ", seatSplitArr);
        for (let index2 = 0; index2 < this.seatmap.length; index2++) {
          const element = this.seatmap[index2];
          if (element.seatRowLabel == seatSplitArr[0]) {
            var seatObj = element.seats[parseInt(seatSplitArr[1]) - 1];
            if (seatObj) {
              console.log("\n\n\nFound Seat to block: ", seatObj);
              seatObj["status"] = "unavailable";
              this.seatmap[index2]["seats"][
                parseInt(seatSplitArr[1]) - 1
              ] = seatObj;
              console.log("\n\n\nSeat Obj", seatObj);
              console.log(
                this.seatmap[index2]["seats"][parseInt(seatSplitArr[1]) - 1]
              );
              break;
            }
          }
        }
      }
    }
  }
}
