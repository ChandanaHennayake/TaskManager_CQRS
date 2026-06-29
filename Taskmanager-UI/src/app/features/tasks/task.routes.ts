import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskCreateComponent } from './components/task-create/task-create.component';
import { TaskEditComponent } from './components/task-edit/task-edit.component';
import { TaskDeleteComponent } from './components/task-delete/task-delete.component';

export const TASK_ROUTES: Routes = [

  { path: '', 
    component: TaskListComponent, 
    canActivate: [authGuard] },

  { path: 'create', 
    component: TaskCreateComponent, 
    canActivate: [authGuard] },

  { path: 'edit/:id', 
    component: TaskEditComponent, 
    canActivate: [authGuard] },

  {
    path: 'delete/:id',
    component: TaskDeleteComponent,
    canActivate: [authGuard],
  },
];
