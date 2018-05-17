import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { FindComponent } from './pages/find/find.component';
import { SignUpComponent } from './auth/sign-up/sign-up.component';
import { SignInComponent } from './auth/sign-in/sign-in.component';
import { TestComponent } from './components/test/test.component';
import { AuthGuardService } from './shared/services/auth-guard.service';
import { EventDetailsComponent } from './pages/event-details/event-details.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'find',
    component: FindComponent
  },
  {
    path: 'find/:keyword',
    component: FindComponent
  },
  {
    path: 'signup',
    component: SignUpComponent
  },
  {
    path: 'signin',
    component: SignInComponent
  },
  {
    path: 'test',
    component: TestComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'event/:id',
    component: EventDetailsComponent
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
