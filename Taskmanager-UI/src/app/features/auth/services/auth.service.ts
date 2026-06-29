import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { LoginRequest } from '../models/login-request.model';
import { LoginResponse } from '../models/login-response.model';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { RegisterRequest } from '../models/register-request.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/users`;

  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(
        `${this.apiUrl}/login`,
        request
      )
      .pipe(
        tap(response => {
          localStorage.setItem('user', JSON.stringify(response.data));
        })
      );
  }

 logout(): Observable<any> {

  return this.http
    .post(`${this.apiUrl}/logout`, {})
    .pipe(
      tap(() => {
        localStorage.removeItem('user');
      })
    );

}

  isLoggedIn(): boolean {
    return localStorage.getItem('user') !== null;
  }

register(request: RegisterRequest): Observable<ApiResponse<number>> {
  return this.http.post<ApiResponse<number>>(
    `${this.apiUrl}/register`,
    request
  );
}

  getCurrentUser(): LoginResponse | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }
}