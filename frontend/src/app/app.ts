import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import {ProductDetailComponent} from './features/products/product-detail/product-detail.component';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent, ProductDetailComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('frontend');
}
