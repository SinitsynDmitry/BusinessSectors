import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Sector {
  id: number;
  name: string;
  path: string;
  isSelected: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class SectorsService {

  private apiUrl = `/api/sectors`;

  constructor(private http: HttpClient) { }

  getSectors(userName: string): Observable<Sector[]> {

    let apiUrl = this.apiUrl;
    if (userName) {
      apiUrl = `${apiUrl}?userName=${userName}`;
    }
    return this.http.get<Sector[]>(apiUrl);
  }
}
