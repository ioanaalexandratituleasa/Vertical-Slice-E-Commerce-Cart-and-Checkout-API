import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../../core/services/product.services';
import { Product } from '../../../core/models/product.models';
import { CartService } from '../../../core/services/cart.services';
import { AuthService } from '../../../core/services/identity.services'; 

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-details.component.html'
})
export class ProductDetailComponent {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private authService = inject(AuthService); 
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  product = signal<Product | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.productService.getProductById(id).subscribe({
      next: (data) => {
        this.product.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Product not found or server error.');
        this.loading.set(false);
      }
    });
  }

  get stockLabel(): string {
    const stock = this.product()?.stock ?? 0;
    if (stock === 0) return 'Out of Stock';
    if (stock < 5) return `Only ${stock} left!`;
    return `In Stock (${stock})`;
  }

  get stockClass(): string {
    const stock = this.product()?.stock ?? 0;
    if (stock === 0) return 'text-danger';
    if (stock < 5) return 'text-warning';
    return 'text-success';
  }

  addToCart(product: Product | null): void {
    if (!product) return;

    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    this.cartService.addToCart(product);
    console.log('Product added from details:', product.title);
  }
}