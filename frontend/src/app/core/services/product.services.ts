import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product.models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly apiUrl = 'https://localhost:7075/api/products';

  constructor(private http: HttpClient) {}

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  getAllProducts(): Observable<Product[]>{
    return this.http.get<Product[]>(this.apiUrl);
  }
}