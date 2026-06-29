import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Task } from '../models/task.model';
import { CreateTaskRequest } from '../models/create-task-request.model';
import { UpdateTaskRequest } from '../models/update-task-request.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiUrl}/tasks`;

  getAll(): Observable<Task[]> {
    return this.http.get<Task[]>(this.baseUrl);
  }

  getById(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateTaskRequest): Observable<any> {
    return this.http.post(this.baseUrl, request);
  }

  update(request: UpdateTaskRequest): Observable<any> {
    return this.http.put(
      `${this.baseUrl}/${request.id}`,
      request
    );
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}