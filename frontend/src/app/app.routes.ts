import { Routes } from '@angular/router';
import { authGuard } from './core/guards/identity.guard';
import { ProductDetailComponent } from './features/products/product-detail/product-detail.component';
import {ProductListComponent} from './features/products/product-list/product-list.component';
import { CartComponent } from './features/cart/cart-component/cart.component';
import { CheckoutComponent } from './features/checkout/checkout.component';
import { RegisterComponent } from './features/identity/register/register.component';
import {LoginComponent} from './features/identity/login/login.component';
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
  },
  {
    path:'register', component:RegisterComponent
  },
  {
    path: 'login', component:LoginComponent
  },
  {
    path: 'cart', component: CartComponent, canActivate: [authGuard]
  },
  {
    path: 'checkout', component:CheckoutComponent, canActivate: [authGuard]
  },
  {
    path: '', redirectTo: 'products', pathMatch:'full'
  }

];