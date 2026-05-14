import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { RegisterRequest, RegisterResponse, LoginRequest, LoginResponse } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'https://localhost:7075/api/identity';

  currentUser = signal<LoginResponse | null>(this.loadFromStorage());

  constructor(private http: HttpClient) {}

  register(data: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, data);
  }

  login(data: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, data).pipe(
      tap(response => {
        localStorage.setItem('auth_token', response.token);
        localStorage.setItem('auth_user', JSON.stringify(response));
        
        this.currentUser.set(response);
      })
    );
  }

  getUserId(): number | null {
    const user = this.currentUser();
    
    if (user) {
      return (user as any).userID || (user as any).id || null;
    }
    const stored = localStorage.getItem('auth_user');
    if (stored) {
      const parsed = JSON.parse(stored);
      return parsed.userID || parsed.id || null;
    }

    return null;
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
    this.currentUser.set(null);
  }

  isLoggedIn(): boolean {
    return this.currentUser() !== null;
  }

  private loadFromStorage(): LoginResponse | null {
    const stored = localStorage.getItem('auth_user');
    try {
      return stored ? JSON.parse(stored) : null;
    } catch (e) {
      console.error("Could not parse auth_user from storage", e);
      return null;
    }
  }
}