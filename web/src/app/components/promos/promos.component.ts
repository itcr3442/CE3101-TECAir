import { Component, Input, OnInit } from '@angular/core';
import { Promo } from 'src/app/interfaces/promo';
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


  constructor(private promoService: PromosService) {
    this.num_promos = 0
    this.promoService.getPromos().subscribe((res) => {
      console.log(res)
      this.promos = res.body as Promo[]
      console.log("Promos:", this.promos)
      this.num_promos = this.promos.length
    })
  }

  ngOnInit(): void {
  }

}
