import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-check-in',
  templateUrl: './check-in.component.html',
  styleUrls: ['./check-in.component.css']
})
export class CheckInComponent implements OnInit {
  seatConfig: any = null;
  seatmap = Array<any>();
  seatChartConfig = {
    showRowsLabel: false,
    showRowWisePricing: false,
    newSeatNoForRow: false
  };
  cart = {
    selectedSeat: null,
    cartId: "",
    eventId: 0
  };

  title = "seat-chart-generator";

  ngOnInit(): void {
    //Process a simple bus layout
    this.seatConfig = [
      {
        seat_map: [
          {
            seat_label: "1",
            layout: "g_____"
          },
          {
            seat_label: "2",
            layout: "gg__gg"
          },
          {
            seat_label: "3",
            layout: "gg__gg"
          },
          {
            seat_label: "4",
            layout: "gg__gg"
          },
          {
            seat_label: "5",
            layout: "gg__gg"
          },
          {
            seat_label: "6",
            layout: "gg__gg"
          },
          {
            seat_label: "7",
            layout: "gg__gg"
          },
          {
            seat_label: "8",
            layout: "gggggg"
          }
        ]
      }
    ];
    this.processSeatChart(this.seatConfig);
    this.blockSeats("7_1,7_2");
  }

  processBooking() {

  }

  public processSeatChart(map_data: any[]) {
    if (map_data.length > 0) {
      var seatNoCounter = 1;
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
