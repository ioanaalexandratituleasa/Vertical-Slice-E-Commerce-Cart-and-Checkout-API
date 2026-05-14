import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductListComponent } from './product-list.component';
import { ProductService } from '../../../core/services/product.services';
import { AuthService } from '../../../core/services/identity.services';
import { CartService } from '../../../core/services/cart.services';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    productServiceSpy = jasmine.createSpyObj('ProductService', ['getAllProducts']);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['isLoggedIn']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const cartSpy = jasmine.createSpyObj('CartService', ['addToCart']);

    productServiceSpy.getAllProducts.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [ProductListComponent],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: CartService, useValue: cartSpy },
        { provide: Router, useValue: routerSpy },
        { 
          provide: ActivatedRoute, 
          useValue: { snapshot: { paramMap: { get: () => '1' } } } 
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('ar trebui să redirecționeze la login dacă utilizatorul nu este autentificat când apasă Add to Cart', () => {
    authServiceSpy.isLoggedIn.and.returnValue(false);
    const mockProduct = { productID: 1, title: 'Test', price: 10, stock: 5, imageP: '', descriptionP: '' };

    component.addToCart(mockProduct);

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });
});