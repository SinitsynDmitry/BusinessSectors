import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Sector {
  id: number;
  name: string;
  path: string;
  order: number;
  isSelected: boolean;
}

export interface SectorNode {
  id: number;
  name: string;
  path: string;
  order: number;
  isSelected: boolean;
  children?: SectorNode[];
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

  saveSectorSelections(data: { name: string, selectedSectorIds: number[] }) {
    return this.http.post<any>('your-api-endpoint', data);
  }


  convertSectorsToSectorNodes(sectors: Sector[]): SectorNode[] {
    // Create map to store all nodes for quick access
    const nodesMap = new Map<number, SectorNode>();

    // First pass: create all nodes without children
    sectors.forEach(sector => {
      nodesMap.set(sector.id, {
        id: sector.id,
        name: sector.name,
        path: sector.path,
        order: sector.order,
        isSelected: sector.isSelected,
        children: []
      });
    });

    // Identify root nodes and build the hierarchy
    const rootNodes: SectorNode[] = [];

    sectors.forEach(sector => {
      const node = nodesMap.get(sector.id)!;

      // Calculate parent ID from path
      const pathParts = sector.path.split('/').filter(Boolean);

      if (pathParts.length <= 1) {
        // This is a root node
        rootNodes.push(node);
      } else if (pathParts.length > 1) {
        const parentId = parseInt(pathParts[pathParts.length - 2]);
        const parentNode = nodesMap.get(parentId);

        if (parentNode) {

          if (!parentNode.children) {
            parentNode.children = [];
          }
          parentNode.children.push(node);
        } else {
          console.warn(`Parent node with ID ${parentId} not found for sector: ${sector.id}`);
          rootNodes.push(node);
        }
      }
    });

    // Sort all nodes by order property
    const sortNodesByOrder = (nodes: SectorNode[]) => {
      nodes.sort((a, b) => a.order - b.order);
      // Recursively sort children
      nodes.forEach(node => {
        if (node.children && node.children.length > 0) {
          sortNodesByOrder(node.children);
        }
      });
    };

    // Sort root nodes and their children
    sortNodesByOrder(rootNodes);

    // Remove empty children arrays
    //const cleanupEmptyChildren = (node: SectorNode) => {
    //  if (node.children && node.children.length === 0) {
    //    delete node.children;
    //  } else if (node.children) {
    //    node.children.forEach(cleanupEmptyChildren);
    //  }
    //};

    //rootNodes.forEach(cleanupEmptyChildren);

    return rootNodes;
  }
}
