import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms'; 
import { CheckoutService } from '../../core/services/checkout.services'; 
import { CartService } from '../../core/services/cart.services'; 

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule], 
  templateUrl: './checkout.component.html'
})
export class CheckoutComponent {
 public cartService = inject(CartService);
 public checkoutService = inject(CheckoutService); 
 public router = inject(Router); 
  cart = this.cartService.cartItems;
  totalPrice = this.cartService.totalPrice;
  address: string = '';

  confirmOrder() {
    const payload = {
      address: this.address,
      items: this.cart().map(item => ({
        productID: item.productID,
        quantity: item.quantity,
        price: item.price
      }))
    };

    this.checkoutService.placeOrder(payload).subscribe({
      next: (response) => {
        alert('Comanda a fost trimisă cu succes!');
        this.cartService.cartItems.set([]); 
        this.router.navigate(['/products']);
      },
      error: (err) => {
        console.error(err);
        alert('Eroare la plasarea comenzii: ' + (err.error || 'Server unreachable'));
      }
    });
  }
}