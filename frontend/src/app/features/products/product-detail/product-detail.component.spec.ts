import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductDetailComponent } from './product-detail.component';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.services';
import { AuthService } from '../../../core/services/identity.services';
import { CartService } from '../../../core/services/cart.services';
import { of } from 'rxjs';

describe('ProductDetailComponent', () => {
  let component: ProductDetailComponent;
  let fixture: ComponentFixture<ProductDetailComponent>;
  let productServiceSpy: jasmine.SpyObj<ProductService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    productServiceSpy = jasmine.createSpyObj('ProductService', ['getProductById']);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['isLoggedIn']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const cartSpy = jasmine.createSpyObj('CartService', ['addToCart']);
    
    await TestBed.configureTestingModule({
      imports: [ProductDetailComponent],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: CartService, useValue: cartSpy },
        { provide: Router, useValue: routerSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { paramMap: { get: () => '1' } }
          }
        }
      ]
    }).compileComponents();

    productServiceSpy.getProductById.and.returnValue(of({
      productID: 1, title: 'Laptop', price: 1000, stock: 3, imageP: '', descriptionP: 'Desc'
    }));

    fixture = TestBed.createComponent(ProductDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('ar trebui să afișeze eticheta corectă pentru stoc scăzut', () => {
    expect(component.stockLabel).toBe('Only 3 left!');
    expect(component.stockClass).toBe('text-warning');
  });

  it('ar trebui să afișeze "Out of Stock" când stocul este 0', () => {
    component.product.set({ productID: 1, title: 'X', price: 10, stock: 0, imageP: '', descriptionP: '' });
    expect(component.stockLabel).toBe('Out of Stock');
    expect(component.stockClass).toBe('text-danger');
  });
});