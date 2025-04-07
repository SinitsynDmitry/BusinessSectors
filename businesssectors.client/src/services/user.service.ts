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

  // Create new user sectors
  createUserSectors(name: string, sectorsIds: string | null): Observable<User> {
    return this.http.post<User>(this.apiUrl, { name, sectorsIds });
  }

  // Update existing user sectors
  updateUserSectors(id: number, sectorsIds: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, { sectorsIds });
  }
}
