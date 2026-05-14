import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { AuthService } from '../../../core/services/identity.services';
import { Router, ActivatedRoute } from '@angular/router'; 
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent, FormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { paramMap: { get: () => '1' } }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('ar trebui să navigheze la produse la login reușit', () => {
    authServiceSpy.login.and.returnValue(of({ 
      token: '123', 
      fName: 'Ion', 
      userID: 1, 
      email: 'ion@test.com' 
    }));
    
    component.email = 'ion@test.com';
    component.password = 'parola';
    component.onSubmit();

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/products']);
    expect(component.loading()).toBeFalse();
  });

  it('ar trebui să afișeze eroare dacă login-ul eșuează', () => {
    authServiceSpy.login.and.returnValue(throwError(() => ({ error: 'Eroare Login' })));
    
    component.onSubmit();

    expect(component.error()).toBe('Eroare Login');
    expect(component.loading()).toBeFalse();
  });
});