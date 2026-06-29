import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';

import { TaskService } from '../../services/task.service';
import { TaskStatus } from '../../enums/task-status.enum';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-task-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './task-create.component.html',
  styleUrl: './task-create.component.scss'
})
export class TaskCreateComponent {

  readonly TaskStatus = TaskStatus;

  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly taskService = inject(TaskService);

  loading = false;

  readonly taskForm = this.fb.nonNullable.group({
    title: [
      '',
      [
        Validators.required,
        Validators.maxLength(200)
      ]
    ],
    description: [
      '',
      [
        Validators.required,
        Validators.maxLength(1000)
      ]
    ],
    status: [
      TaskStatus.Pending,
      Validators.required
    ]
  });

  get title() {
    return this.taskForm.controls.title;
  }

  get description() {
    return this.taskForm.controls.description;
  }

  get status() {
    return this.taskForm.controls.status;
  }

  onSubmit(): void {

    if (this.taskForm.invalid) {

      this.taskForm.markAllAsTouched();

      return;

    }

    this.loading = true;

    this.taskService
  .create(this.taskForm.getRawValue())
  .pipe(
    finalize(() => this.loading = false)
  )
  .subscribe({

    next: () => {

      Swal.fire({
        icon: 'success',
        title: 'Success',
        text: 'Task created successfully.',
        confirmButtonColor: '#0d6efd'
      }).then(() => {

        this.router.navigate(['/tasks']);

      });

    },

    error: error => {

      console.error('Failed to create task.', error);

      Swal.fire({
        icon: 'error',
        title: 'Create Failed',
        text: 'Unable to create the task. Please try again.',
        confirmButtonColor: '#dc3545'
      });

    }

  });

  }

}