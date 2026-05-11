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
    if (existingItem.quantity < product.stock) {
      existingItem.quantity++;
      this.cartItems.set([...items]);
    } else {
      alert(`Sorry, only ${product.stock} items available in stock!`);
    }
  } else {
    if (product.stock > 0) {
      this.cartItems.set([...items, { ...product, quantity: 1 }]);
    }
  }
}

  removeFromCart(productId: number) {
    this.cartItems.set(this.cartItems().filter(i => i.productID !== productId));
  }

  decreaseQuantity(productId: number) {
  const items = this.cartItems();
  const index = items.findIndex(i => i.productID === productId);

  if (index !== -1) {
    const updatedItems = [...items];
    if (updatedItems[index].quantity > 1) {
      updatedItems[index] = { 
        ...updatedItems[index], 
        quantity: updatedItems[index].quantity - 1 
      };
      this.cartItems.set(updatedItems);
    } else {
      this.removeFromCart(productId);
    }
  }
}
}