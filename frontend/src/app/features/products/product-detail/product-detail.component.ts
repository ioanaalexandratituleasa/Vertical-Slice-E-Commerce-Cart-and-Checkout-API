import { Component, OnInit,inject, signal } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../../core/services/product.services';
import { Product } from '../../../core/models/product.models';
import { CartService } from '../../../core/services/cart.services';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-detail.component.css'
})
export class ProductDetailComponent implements OnInit {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  product = signal<Product | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private route: ActivatedRoute,
  ) {}
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

  addToCart(product: Product | null) {
    if (product) {
      this.cartService.addToCart(product);
      // Opțional: un feedback vizual (ex: alert sau toast)
      console.log('Adăugat din detalii:', product.title);
    }
  }
}
