import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-promos',
  templateUrl: './promos.component.html',
  styleUrls: ['./promos.component.css']
})
export class PromosComponent implements OnInit {

  @Input() fontSize: number = 1;

  constructor() { }

  ngOnInit(): void {
  }

}
