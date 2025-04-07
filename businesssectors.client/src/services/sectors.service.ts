import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

 interface Sector {
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

  getSectorNodes(userId: number): Observable<SectorNode[]> {

    let apiUrl = this.apiUrl;
    if (userId) {
      apiUrl = `${apiUrl}?userId=${userId}`;
    }

    return this.http.get<Sector[]>(apiUrl).pipe(
      map((sectors: Sector[]) => this.convertSectorsToSectorNodes(sectors))
    );
  }

  private convertSectorsToSectorNodes(sectors: Sector[]): SectorNode[] {
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

    return rootNodes;
  }
}
