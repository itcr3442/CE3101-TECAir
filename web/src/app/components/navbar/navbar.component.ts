import { Component, OnInit } from '@angular/core';
import { RoleLevels } from 'src/app/constants/auth.constants';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  logged: boolean;
  role: number;

  constructor(private authService: AuthService) {
    this.logged = authService.isLoggedIn()
    this.role = 0
    this.authService.getRole().then(role => this.role = role)
  }

  ngOnInit(): void {
    this.logged = this.authService.isLoggedIn()
  }

  // Este get es para poder usarlo dentro de template
  public get RoleLevels(): typeof RoleLevels {
    return RoleLevels;
  }

}
