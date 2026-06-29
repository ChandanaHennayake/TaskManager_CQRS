import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  ActivatedRoute,
  Router,
  RouterModule
} from '@angular/router';
import { finalize } from 'rxjs';

import { TaskService } from '../../services/task.service';
import { TaskStatus } from '../../enums/task-status.enum';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-task-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './task-edit.component.html',
  styleUrl: './task-edit.component.scss'
})
export class TaskEditComponent implements OnInit {

  readonly TaskStatus = TaskStatus;

  readonly statuses = [
    {
      value: TaskStatus.Pending,
      label: 'Pending'
    },
    {
      value: TaskStatus.InProgress,
      label: 'In Progress'
    },
    {
      value: TaskStatus.Completed,
      label: 'Completed'
    }
  ];

  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly taskService = inject(TaskService);

  loading = false;

  private taskId = 0;

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

  ngOnInit(): void {

    const id = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!id || Number.isNaN(id)) {
      this.router.navigate(['/tasks']);
      return;
    }

    this.taskId = id;

    this.loadTask();

  }

  private loadTask(): void {

    this.loading = true;

    this.taskService
      .getById(this.taskId)
      .pipe(
        finalize(() => this.loading = false)
      )
      .subscribe({

        next: task => {

          this.taskForm.setValue({

            title: task.title,

            description: task.description,

            status: task.status

          });

        },

        error: error => {

          console.error('Failed to load task.', error);

          this.router.navigate(['/tasks']);

        }

      });

  }

  onSubmit(): void {

    if (this.taskForm.invalid) {

      this.taskForm.markAllAsTouched();

      return;

    }

    this.loading = true;

    const request = {

      id: this.taskId,

      ...this.taskForm.getRawValue()

    };

    this.taskService
      .update(request)
      .pipe(
        finalize(() => this.loading = false)
      )
      .subscribe({

       next: () => {

  Swal.fire({
    icon: 'success',
    title: 'Success',
    text: 'Task updated successfully.',
    confirmButtonColor: '#0d6efd'
  }).then(() => {

    this.router.navigate(['/tasks']);

  });

},

        error: error => {

          console.error('Failed to update task.', error);

        }

      });

  }

}