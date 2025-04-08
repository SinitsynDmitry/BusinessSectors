import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface User {
  id: number;
  name: string;
  sectorsIds?: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'api/usersectors';

  constructor(private http: HttpClient) { }

  // Get user sectors by name
  getUserSectors(name: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${encodeURIComponent(name)}`);
  }

  // Update existing user sectors
  updateUserSectors(id: number, sectorsIds: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, { sectorsIds });
  }
}
