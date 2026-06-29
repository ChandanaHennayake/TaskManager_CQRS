import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-task-delete',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './task-delete.component.html',
  styleUrl: './task-delete.component.scss'
})
export class TaskDeleteComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private taskService = inject(TaskService);

  taskId = 0;
  task?: Task;
  loading = false;

  ngOnInit(): void {

    this.taskId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    this.loadTask();
  }

  loadTask(): void {

    this.loading = true;

    this.taskService.getById(this.taskId)
      .subscribe({
        next: (task) => {
          this.task = task;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
  }

  deleteTask(): void {

  Swal.fire({
    title: 'Delete Task?',
    text: 'This action cannot be undone.',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#dc3545',
    cancelButtonColor: '#6c757d',
    confirmButtonText: 'Yes, Delete',
    cancelButtonText: 'Cancel'
  }).then(result => {

    if (!result.isConfirmed) {
      return;
    }

    this.loading = true;

    this.taskService
      .delete(this.taskId)
      .subscribe({

        next: () => {

          this.loading = false;

          Swal.fire({
            icon: 'success',
            title: 'Deleted!',
            text: 'Task deleted successfully.',
            confirmButtonColor: '#0d6efd'
          }).then(() => {

            this.router.navigate(['/tasks']);

          });

        },

        error: error => {

          this.loading = false;

          console.error(error);

          Swal.fire({
            icon: 'error',
            title: 'Delete Failed',
            text: 'Unable to delete the task. Please try again.',
            confirmButtonColor: '#dc3545'
          });

        }

      });

  });

}
}