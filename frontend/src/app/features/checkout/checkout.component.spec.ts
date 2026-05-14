import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CheckoutComponent } from './checkout.component';
import { CheckoutService } from '../../core/services/checkout.services';
import { CartService } from '../../core/services/cart.services';
import { AuthService } from '../../core/services/identity.services';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { provideRouter } from '@angular/router';

describe('CheckoutComponent', () => {
  let component: CheckoutComponent;
  let fixture: ComponentFixture<CheckoutComponent>;
  let checkoutServiceSpy: jasmine.SpyObj<CheckoutService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const cSpy = jasmine.createSpyObj('CheckoutService', ['placeOrder']);
    const aSpy = jasmine.createSpyObj('AuthService', ['getUserId']);

    await TestBed.configureTestingModule({
      imports: [CheckoutComponent, FormsModule],
      providers: [
        { provide: CheckoutService, useValue: cSpy },
        { provide: AuthService, useValue: aSpy },
        CartService,
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CheckoutComponent);
    component = fixture.componentInstance;
    checkoutServiceSpy = TestBed.inject(CheckoutService) as jasmine.SpyObj<CheckoutService>;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  it('butonul "Place Order" ar trebui să fie dezactivat dacă adresa este goală', () => {
    component.address = '';
    fixture.detectChanges();
    const button = fixture.nativeElement.querySelector('button');
    expect(button.disabled).toBeTrue();
  });

  it('ar trebui să apeleze placeOrder și să golească coșul la succes', () => {
    authServiceSpy.getUserId.and.returnValue(1);
    checkoutServiceSpy.placeOrder.and.returnValue(of({ message: 'Success' }));
    
    component.cartService.cartItems.set([{ productID: 1, title: 'P1', price: 10, quantity: 1, stock: 5, descriptionP: '', imageP: '' }]);
    component.address = 'Strada Test 123';

    component.confirmOrder();

    expect(checkoutServiceSpy.placeOrder).toHaveBeenCalled();
    expect(component.cartService.cartItems().length).toBe(0); 
  });
});