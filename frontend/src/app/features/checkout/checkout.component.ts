import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms'; 
import { CheckoutService } from '../../core/services/checkout.services'; 
import { CartService } from '../../core/services/cart.services'; 
import { AuthService } from '../../core/services/identity.services';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule], 
  templateUrl: './checkout.component.html'
})
export class CheckoutComponent {
  public cartService = inject(CartService);
  private checkoutService = inject(CheckoutService); 
  private authService = inject(AuthService);
  private router = inject(Router); 

  cart = this.cartService.cartItems;
  address: string = '';

  confirmOrder() {
    const userId = this.authService.getUserId();

    if (!userId) {
      alert('Error: You must be logged in to place an order.');
      return;
    }

    const payload = {
      UserID: userId,
      Address: this.address,
      Items: this.cart().map(item => ({
        ProductID: item.productID,
        Quantity: item.quantity,
        TotalPrice: item.price 
      }))
    };

    console.log("Sending payload:", payload);

    this.checkoutService.placeOrder(payload).subscribe({
      next: (response) => {
        alert('Comandă finalizată cu succes!');
        this.cartService.cartItems.set([]); 
        this.router.navigate(['/products']);
      },
      error: (err) => {
        console.error("Server Error:", err);
        alert('Eroare: ' + (err.error || 'Check console for details'));
      }
    });
  }
}