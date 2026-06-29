import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs';

import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task.model';
import { TaskStatus } from '../../enums/task-status.enum';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule
  ],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss'
})
export class TaskListComponent implements OnInit {

  readonly TaskStatus = TaskStatus;

  private readonly taskService = inject(TaskService);

  tasks: Task[] = [];
  filteredTasks: Task[] = [];

  loading = false;

  searchQuery = '';
  statusFilter = '';

  private readonly statusLabels: Record<number, string> = {
    [TaskStatus.Pending]: 'Pending',
    [TaskStatus.InProgress]: 'In Progress',
    [TaskStatus.Completed]: 'Completed'
  };

  private readonly statusClasses: Record<number, string> = {
    [TaskStatus.Pending]: 'badge-todo',
    [TaskStatus.InProgress]: 'badge-inprog',
    [TaskStatus.Completed]: 'badge-done'
  };

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {

    this.loading = true;

    this.taskService
      .getAll()
      .pipe(
        finalize(() => (this.loading = false))
      )
      .subscribe({

        next: (tasks) => {

          this.tasks = tasks;

          this.applyFilter();

        },

        error: (error) => {

          console.error('Failed to load tasks.', error);

        }

      });

  }

  applyFilter(): void {

    const query = this.searchQuery
      .trim()
      .toLowerCase();

    const status =
      this.statusFilter === ''
        ? null
        : Number(this.statusFilter);

    this.filteredTasks = this.tasks.filter(task => {

      const matchesSearch =

        task.title
          .toLowerCase()
          .includes(query)

        ||

        (task.description ?? '')
          .toLowerCase()
          .includes(query);

      const matchesStatus =

        status === null

        ||

        task.status === status;

      return matchesSearch && matchesStatus;

    });

  }

  clearFilters(): void {

    this.searchQuery = '';

    this.statusFilter = '';

    this.filteredTasks = [...this.tasks];

  }

  countByStatus(status: TaskStatus): number {

    return this.tasks.filter(t => t.status === status).length;

  }

  getStatusLabel(status: TaskStatus): string {

    return this.statusLabels[status] ?? 'Unknown';

  }

  getStatusClass(status: TaskStatus): string {

    return this.statusClasses[status] ?? 'badge-todo';

  }

  get totalTasks(): number {

    return this.tasks.length;

  }

  get pendingTasks(): number {

    return this.countByStatus(TaskStatus.Pending);

  }

  get inProgressTasks(): number {

    return this.countByStatus(TaskStatus.InProgress);

  }

  get completedTasks(): number {

    return this.countByStatus(TaskStatus.Completed);

  }

}