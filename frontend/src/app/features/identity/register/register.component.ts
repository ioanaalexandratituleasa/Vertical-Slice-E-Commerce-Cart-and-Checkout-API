import {Component, signal} from '@angular/core';
import{FormsModule} from '@angular/forms';
import{CommonModule} from "@angular/common";
import{Router, RouterModule} from '@angular/router';
import{AuthService} from '../../../core/services/identity.services';

@Component({
    selector:'app-register',
    standalone: true,
    imports:[CommonModule, FormsModule, RouterModule],
    templateUrl: './register.component.html'
})

export class RegisterComponent{
    fName = '';
    lName = '';
    email = '';
    password='';

    loading = signal(false);
    error = signal<string | null>(null);
    success = signal<string | null>(null);

    constructor(
        private authService: AuthService,
        private router: Router
    ){}

    onSubmit():void{
        this.loading.set(true);
    this.error.set(null);

    this.authService.register({
      fName: this.fName,
      lName: this.lName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: (res) => {
        this.success.set(`Cont creat cu succes! Bine ai venit, ${res.fName}!`);
        this.authService.login({
        email: this.email,
        password: this.password
      }).subscribe({
        next: () => {
          this.loading.set(false);
          setTimeout(() => this.router.navigate(['/products']), 2000);
        },
        error: () => {
          this.loading.set(false);
          setTimeout(() => this.router.navigate(['/login']), 2000);
        }
      });
    },
    error: (err) => {
      this.error.set(err.error || 'A apărut o eroare.');
      this.loading.set(false);
    }
  });
  }
}