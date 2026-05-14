import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RegisterComponent } from './register.component';
import { AuthService } from '../../../core/services/identity.services';
import { Router, ActivatedRoute } from '@angular/router'; 
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['register', 'login']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [RegisterComponent, FormsModule],
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

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('ar trebui să încerce login automat după register reușit', fakeAsync(() => {
    authServiceSpy.register.and.returnValue(of({ 
      fName: 'Ion', 
      lName: 'Popescu', 
      email: 'ion@test.com', 
      eMail: 'ion@test.com', 
      userID: 1 
    }));
    
    authServiceSpy.login.and.returnValue(of({ 
      token: '123', 
      fName: 'Ion', 
      email: 'ion@test.com', 
      userID: 1 
    }));

    component.onSubmit();
    
    expect(authServiceSpy.register).toHaveBeenCalled();
    tick(); 
    
    expect(authServiceSpy.login).toHaveBeenCalled();
    tick(2000); 
    
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/products']);
  }));
});