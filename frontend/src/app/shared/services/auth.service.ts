import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationUserModel } from '../models/authenticationUser.model';
import { Router } from '@angular/router';
import { tokenNotExpired } from 'angular2-jwt';

@Injectable()
export class AuthService {
  constructor(private _http: HttpClient, private router: Router) {}
  url = 'Authorize';

  public getToken(): string {
    return localStorage.getItem('token');
  }

  public isAuthenticated(): boolean {
    const token = this.getToken();
    if (token === 'null') {
      return false;
    }
    return tokenNotExpired(null, token);
  }

  signUpUser(email: string, password: string) {
    const user: AuthenticationUserModel = new AuthenticationUserModel(
      email,
      password
    );
    return this._http.post(this.url + '/Register', user);
  }
  signInUser(email: string, password: string) {
    const user: AuthenticationUserModel = new AuthenticationUserModel(
      email,
      password
    );
    return this._http.post(this.url, user).subscribe(
      (token: string) => {
        this.router.navigate(['/']);
        localStorage.setItem('token', token['token']);
      },
      error => console.log(error)
    );
  }

  logout() {
    localStorage.clear();
    this.router.navigate(['/']);
  }
}
