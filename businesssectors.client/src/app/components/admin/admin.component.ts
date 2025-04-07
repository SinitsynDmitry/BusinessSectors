import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatTreeFlatDataSource, MatTreeFlattener, MatTreeModule } from '@angular/material/tree';
import { SectorNode, SectorsService } from '../../../services/sectors.service';

import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { EditNodeDialogComponent } from './edit-node-dialog.component';
import { tap, catchError, of, finalize } from 'rxjs';

/** Flat node with expandable and level information */
interface FlatNode {
  expandable: boolean;
  name: string;
  level: number;
  id: number;
  path: string;
  order: number;
  isSelected: boolean;
}

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTreeModule,
    MatIconModule
  ]
})
export class AdminComponent implements OnInit {
  private _transformer = (node: SectorNode, level: number): FlatNode => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      level: level,
      id: node.id,
      path: node.path,
      order: node.order,
      isSelected: node.isSelected
    };
  };

  treeControl = new FlatTreeControl<FlatNode>(
    node => node.level,
    node => node.expandable
  );

  treeFlattener = new MatTreeFlattener(
    this._transformer,
    node => node.level,
    node => node.expandable,
    node => node.children
  );

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  // Store original data for restore functionality
  originalSectorNodes: SectorNode[] = [];
  currentSectorNodes: SectorNode[] = [];

  addedSectorNodes: SectorNode[] = [];
  removedSectorNodes: SectorNode[] = [];
  editedSectorNodes: SectorNode[] = [];

  // Loading state indicators
  isLoading = false;
  isSaving = false;

  constructor(
    private sectorsService: SectorsService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit() {
    this.loadSectorNodes();
  }

  loadSectorNodes() {
    this.isLoading = true;
    this.sectorsService.getSectorNodes(0)
      .pipe(
        tap(nodes => {
          this.originalSectorNodes = nodes;
          // Create a deep copy for working data
          this.currentSectorNodes = JSON.parse(JSON.stringify(this.originalSectorNodes));
          this.dataSource.data = this.currentSectorNodes;
          this.treeControl.expandAll();
        }),
        finalize(() => this.isLoading = false),
        catchError(error => {
          console.error('Error loading sector nodes:', error);
          this.showError('Failed to load sectors');
          return of([]);
        })
      )
      .subscribe();
  }

  hasChild(_: number, node: FlatNode): boolean {
    return node.expandable;
  }

  // Add a new node after the selected node
  addNewNode(node: FlatNode) {
    // Find the parent node to add a child
    const parentPath = node.path.substring(0, node.path.lastIndexOf('/'));
    const newNodeId = this.getNextNodeId();
    const newNodeOrder = this.getNextOrderInLevel(node);

    // Create new node
    const newNode: SectorNode = {
      id: newNodeId,
      name: 'New Sector',
      path: `${parentPath}/${newNodeId}/`,
      order: newNodeOrder,
      isSelected: false
    };
    console.log("new node created", newNode);

    // Add the new node to the tree structure
    this.addNodeToTree(this.currentSectorNodes, parentPath, newNode);

    // Update the data source
    this.dataSource.data = [...this.currentSectorNodes];
    // Open edit dialog for the new node
    this.editNodeName({ ...newNode, level: node.level, expandable: false });
    this.addedSectorNodes.push(newNode);
    console.log("new node renamed", newNode);
    this.treeControl.expandAll();
  }

  // Delete node and all its children
  deleteNode(node: FlatNode) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '300px',
      data: {
        title: 'Confirm Deletion',
        message: `Are you sure you want to delete "${node.name}" and all its children?`,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteNodeFromTree(this.currentSectorNodes, node.path);
        this.dataSource.data = [...this.currentSectorNodes];
        this.showSuccess(`"${node.name}" has been deleted`);
        this.treeControl.expandAll();
      }
    });
  }

  // Open dialog to edit node name
  editNodeName(node: FlatNode) {
    const dialogRef = this.dialog.open(EditNodeDialogComponent, {
      width: '250px',
      data: { name: node.name }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const oldName = node.name;
        // Update node name in the tree structure
        this.updateNodeNameInTree(this.currentSectorNodes, node.path, result);
        this.dataSource.data = [...this.currentSectorNodes];
        this.showSuccess(`"${oldName}" renamed to "${result}"`);
        this.treeControl.expandAll();
      }
    });
  }

  // Helper method to add a node to the tree
  private addNodeToTree(nodes: SectorNode[], parentPath: string, newNode: SectorNode) {
    if (parentPath === '') {
      // Add to root level
      nodes.push(newNode);
      return;
    }

    for (const node of nodes) {
      if (node.path === parentPath + '/') {
        // Found parent node
        if (!node.children) {
          node.children = [];
        }
        node.children.push(newNode);
        return;
      }

      if (node.children && node.path.startsWith(parentPath.substring(0, parentPath.lastIndexOf('/')))) {
        this.addNodeToTree(node.children, parentPath, newNode);
      }
    }
  }

  // Helper method to delete a node from the tree
  private deleteNodeFromTree(nodes: SectorNode[], nodePath: string): boolean {
    let deleted: SectorNode[];
    for (let i = 0; i < nodes.length; i++) {
      if (nodes[i].path === nodePath) {
        // Found the node to delete
        deleted = nodes.splice(i, 1);
        console.log("deleted", deleted);
        this.removedSectorNodes.push(...deleted);
        console.log("removedSectorNodes", this.removedSectorNodes);
        return true;
      }

      if (nodes[i].children) {
        if (this.deleteNodeFromTree(nodes[i].children!, nodePath)) {
          return true;
        }
      }
    }
    return false;
  }

  // Helper method to update a node name in the tree
  private updateNodeNameInTree(nodes: SectorNode[], nodePath: string, newName: string): boolean {
    for (let node of nodes) {
      if (node.path === nodePath) {
        // Found the node to update
        node.name = newName;
        this.editedSectorNodes.push(node);
        return true;
      }

      if (node.children) {
        if (this.updateNodeNameInTree(node.children, nodePath, newName)) {
          return true;
        }
      }
    }
    return false;
  }

  // Generate a new unique ID for a new node
  private getNextNodeId(): number {
    // Simple implementation - in real app, you might want to get this from a service
    let maxId = 0;

    const findMaxId = (nodes: SectorNode[]) => {
      for (const node of nodes) {
        if (node.id > maxId) {
          maxId = node.id;
        }
        if (node.children) {
          findMaxId(node.children);
        }
      }
    };

    findMaxId(this.currentSectorNodes);
    return maxId + 1;
  }

  // Get the next order number for a new node at the same level
  private getNextOrderInLevel(node: FlatNode): number {
    // Find siblings and get the max order
    let maxOrder = 0;

    const findMaxOrderInLevel = (nodes: SectorNode[], targetLevel: string) => {
      for (const node of nodes) {
        const parentPath = node.path.substring(0, node.path.lastIndexOf('/'));
        if (parentPath === targetLevel) {
          if (node.order > maxOrder) {
            maxOrder = node.order;
          }
        }

        if (node.children) {
          findMaxOrderInLevel(node.children, targetLevel);
        }
      }
    };

    const parentPath = node.path.substring(0, node.path.lastIndexOf('/'));
    findMaxOrderInLevel(this.currentSectorNodes, parentPath);

    return maxOrder + 1;
  }

  // Save changes
  saveSectors() {
    this.isSaving = true;
    // Convert tree structure to flat array of sectors
    const sectors = this.flattenSectorTree(this.currentSectorNodes);
    console.log("addedSectorNodes", this.addedSectorNodes);
    console.log("removedSectorNodes", this.removedSectorNodes);
    console.log("editedSectorNodes", this.editedSectorNodes);
    // Call service to save sectors
    this.sectorsService.saveSectors(sectors).subscribe({
      next: () => {
        this.showSuccess('Sectors saved successfully');
        // Update original data after saving
        this.originalSectorNodes = JSON.parse(JSON.stringify(this.currentSectorNodes));
      },
      complete: () => { this.isSaving = false; },
      error: (err) => {
        console.error('Error saving sectors', err);
        this.showError('Error saving sectors');
      }
    });
  }

  // Restore from original data
  restoreSectors() {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '300px',
      data: {
        title: 'Confirm Restore',
        message: 'Are you sure you want to discard all changes?',
        confirmButtonText: 'Restore',
        cancelButtonText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.currentSectorNodes = JSON.parse(JSON.stringify(this.originalSectorNodes));
        this.dataSource.data = this.currentSectorNodes;
        this.showSuccess('Changes discarded, original state restored');
        this.treeControl.expandAll();
      }
    });
  }

  // Convert tree structure to flat array of sectors
  private flattenSectorTree(nodes: SectorNode[]): any[] {
    let result: any[] = [];

    const flatten = (nodes: SectorNode[]) => {
      for (const node of nodes) {
        result.push({
          id: node.id,
          name: node.name,
          path: node.path,
          order: node.order,
          isSelected: false // Default to false as specified
        });

        if (node.children) {
          flatten(node.children);
        }
      }
    };

    flatten(nodes);
    return result;
  }

  // Helper methods for user feedback
  showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['success-snackbar']
    });
  }

  showError(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }
}

