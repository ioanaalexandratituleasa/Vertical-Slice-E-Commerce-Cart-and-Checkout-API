import{Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import{RegisterRequest, RegisterResponse} from '../models/user.model';

@Injectable({providedIn: 'root'})
export class AuthService{
    private readonly apiUrl = 'https://localhost:7075/api/identity';

    constructor(private http: HttpClient){}

    register(data : RegisterRequest): Observable<RegisterResponse>{
        return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, data);
    }
}