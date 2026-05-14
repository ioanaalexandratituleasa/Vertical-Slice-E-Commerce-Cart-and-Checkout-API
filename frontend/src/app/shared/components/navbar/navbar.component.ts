import {Component, inject} from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/identity.services';
@Component({
  selector: 'app-navbar', 
  standalone: true,     
  imports: [CommonModule,RouterModule], 
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})

export class NavbarComponent { 
 constructor(public authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}