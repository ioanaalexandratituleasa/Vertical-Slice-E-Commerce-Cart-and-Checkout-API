import { TestBed } from '@angular/core/testing';
import { CartService } from './cart.services';
import { Product } from '../models/product.models';

describe('CartService', () => {
  let service: CartService;

  const mockProduct: Product = {
    productID: 1,
    title: 'Test Product',
    price: 100,
    stock: 5,
    descriptionP: '',
    imageP: ''
  };

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CartService);
  });

  it('ar trebui să adauge un produs nou în coș', () => {
    service.addToCart(mockProduct);
    expect(service.cartItems().length).toBe(1);
    expect(service.totalItems()).toBe(1);
  });

  it('ar trebui să crească cantitatea dacă produsul există deja și este în stoc', () => {
    service.addToCart(mockProduct);
    service.addToCart(mockProduct); 
    
    expect(service.cartItems()[0].quantity).toBe(2);
    expect(service.totalPrice()).toBe(200);
  });

  it('NU ar trebui să depășească stocul disponibil', () => {
    const limitedProduct = { ...mockProduct, stock: 1 };
    service.addToCart(limitedProduct);
    service.addToCart(limitedProduct); 

    expect(service.cartItems()[0].quantity).toBe(1);
  });

  it('ar trebui să elimine produsul dacă cantitatea scade sub 1', () => {
    service.addToCart(mockProduct);
    service.decreaseQuantity(mockProduct.productID);
    expect(service.cartItems().length).toBe(0);
  });
});