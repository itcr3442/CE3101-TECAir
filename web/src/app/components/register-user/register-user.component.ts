import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RoleLevels } from 'src/app/constants/auth.constants';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register-user',
  templateUrl: './register-user.component.html',
  styleUrls: ['./register-user.component.css']
})
export class RegisterUserComponent implements OnInit {
  registerForm = new FormGroup({
    id: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
    password: new FormControl('', [Validators.required]),
    firstName: new FormControl('', [Validators.required]),
    lastName: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required]),
    phone: new FormControl('', [Validators.required, Validators.pattern('[0-9]*')]),
    isStudent: new FormControl(''),
    university: new FormControl(''),
    studentId: new FormControl('')
  })
  message: string = ""
  isStudent: boolean;

  constructor(
    private authService: AuthService,
  ) {
    this.isStudent = false
  }

  ngOnInit(): void {
  }

  // Este get es para poder usarlo dentro de template
  public get RoleLevels(): typeof RoleLevels {
    return RoleLevels;
  }

  get id() {
    return this.registerForm.controls['id'].value
  }

  get password() {
    return this.registerForm.controls['password'].value
  }

  get university() {
    return this.registerForm.controls['university'].value
  }

  get studentId() {
    return this.registerForm.controls['studentId'].value
  }

  onStudentChange(event: Event) {
    let target = event.target as HTMLInputElement

    this.isStudent = target.checked

    if (this.isStudent) {
      let studentId = this.registerForm.get('studentId')
      let university = this.registerForm.get('university')
      studentId?.setValidators(Validators.required);
      university?.setValidators(Validators.required);
      studentId?.updateValueAndValidity();
      university?.updateValueAndValidity();
    }

  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.message = ""
    }
    else {
      this.message = "Por favor verifique que ingresó todos los campos correctamente y su cédula y # de tel solo contiene dígitos";
    }
  }
}