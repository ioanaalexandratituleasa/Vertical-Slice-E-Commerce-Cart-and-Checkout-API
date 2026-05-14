import { Routes } from '@angular/router';
import { ProductDetailComponent } from './features/products/product-detail/product-detail.component';
import {ProductListComponent} from './features/products/product-list/product-list.component';
import { CartComponent } from './features/cart/cart-component/cart.component';
import { CheckoutComponent } from './features/checkout/checkout.component';
export const routes: Routes = [
  {
    path: 'products/:id', component: ProductDetailComponent
  },
  {
    path: 'products', component:ProductListComponent
  },
  {
    path:'cart', component: CartComponent
  },
  {
    path:'checkout', component: CheckoutComponent
  }

];