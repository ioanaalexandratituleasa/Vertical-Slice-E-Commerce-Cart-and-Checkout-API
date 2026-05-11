import { Routes } from '@angular/router';
import { ProductDetailComponent } from './features/products/product-detail/product-detail.component';

export const routes: Routes = [
  {
    path: 'products/:id', component: ProductDetailComponent
  }
];