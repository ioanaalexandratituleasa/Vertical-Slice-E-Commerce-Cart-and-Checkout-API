import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../../core/services/cart.services'; 

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.component.html'
})
export class CartComponent {
  private cartService = inject(CartService);
  cart = this.cartService.cartItems; 
  totalPrice = this.cartService.totalPrice;

  removeItem(productId: number) {
    this.cartService.removeFromCart(productId);
  }
}