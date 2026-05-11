import { Injectable, signal, computed } from '@angular/core';
import { Product } from '../models/product.models';

export interface CartItem extends Product {
  quantity: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {
 
  cartItems = signal<CartItem[]>([]);

  totalItems = computed(() => 
    this.cartItems().reduce((acc, item) => acc + item.quantity, 0)
  );

  totalPrice = computed(() => 
    this.cartItems().reduce((acc, item) => acc + (item.price * item.quantity), 0)
  );

  addToCart(product: Product) {
    const items = this.cartItems();
    const existingItem = items.find(i => i.productID === product.productID);

    if (existingItem) {
      existingItem.quantity++;
      this.cartItems.set([...items]);
    } else {
      this.cartItems.set([...items, { ...product, quantity: 1 }]);
    }
  }

  removeFromCart(productId: number) {
    this.cartItems.set(this.cartItems().filter(i => i.productID !== productId));
  }
}