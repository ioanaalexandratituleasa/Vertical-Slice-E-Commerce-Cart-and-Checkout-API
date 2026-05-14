import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './identity.services';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify(); // Verifică să nu existe cereri HTTP nerezolvate
  });

  it('ar trebui să fie creat', () => {
    expect(service).toBeTruthy();
  });

  it('ar trebui să salveze token-ul în localStorage la login', () => {
    const mockResponse = { token: 'abc-123', fName: 'Test', userID: 1 };

    service.login({ email: 'test@test.com', password: '123' }).subscribe(res => {
      expect(res.token).toBe('abc-123');
      expect(localStorage.getItem('auth_token')).toBe('abc-123');
    });

    const req = httpMock.expectOne('https://localhost:7075/api/identity/login');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('ar trebui să șteargă datele la logout', () => {
    localStorage.setItem('auth_token', 'fake-token');
    service.logout();
    expect(localStorage.getItem('auth_token')).toBeNull();
    expect(service.currentUser()).toBeNull();
  });
});