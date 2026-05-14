import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ActivatedRoute, RouterModule } from '@angular/router';
import { ProductService } from '../../../core/services/product.services';
import { Product } from '../../../core/models/product.models';
import { CartService } from '../../../core/services/cart.services';
import { AuthService } from '../../../core/services/identity.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-list.component.html'
})
export class ProductListComponent {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private authService = inject(AuthService);  
  private router = inject(Router);          
  private route = inject(ActivatedRoute);

  products = signal<Product[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.productService.getAllProducts().subscribe({
      next: (data) => {
        this.products.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
  
  addToCart(product: Product): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    this.cartService.addToCart(product);
    console.log('Product added:', product.title);
  }
}