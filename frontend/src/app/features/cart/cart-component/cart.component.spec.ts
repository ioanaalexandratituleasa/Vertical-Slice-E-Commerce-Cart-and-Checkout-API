import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CartComponent } from './cart.component';
import { CartService } from '../../../core/services/cart.services';
import { provideRouter } from '@angular/router';

describe('CartComponent', () => {
  let component: CartComponent;
  let fixture: ComponentFixture<CartComponent>;
  let cartService: CartService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartComponent],
      providers: [CartService, provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(CartComponent);
    component = fixture.componentInstance;
    cartService = TestBed.inject(CartService);
    fixture.detectChanges();
  });

  it('ar trebui să afișeze mesajul "Your cart is empty" când nu sunt produse', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.alert-info')?.textContent).toContain('Your cart is empty');
  });

  it('ar trebui să afișeze tabelul când există produse în coș', () => {
    cartService.cartItems.set([{ productID: 1, title: 'Laptop', price: 1000, quantity: 1, stock: 5, descriptionP: '', imageP: '' }]);
    fixture.detectChanges(); 

    const table = fixture.nativeElement.querySelector('table');
    expect(table).toBeTruthy();
    expect(fixture.nativeElement.textContent).toContain('Laptop');
  });
});