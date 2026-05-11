import { Routes } from '@angular/router';
import { ProductDetailComponent } from './features/products/product-detail/product-detail.component';
import {ProductListComponent} from './features/products/product-list/product-list.component';
export const routes: Routes = [
  {
    path: 'products/:id', component: ProductDetailComponent
  },
  {
    path: 'products', component:ProductListComponent
  }

];