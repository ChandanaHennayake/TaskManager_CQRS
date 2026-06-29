import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadChildren: () =>
      import('./features/auth/auth.routes')
        .then(m => m.AUTH_ROUTES)
  },
  {
    path: 'tasks',
    loadChildren: () =>
      import('./features/tasks/task.routes')
        .then(m => m.TASK_ROUTES)
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];