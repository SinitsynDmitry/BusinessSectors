// sector-tree.component.ts
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatTreeFlatDataSource, MatTreeFlattener, MatTreeModule } from '@angular/material/tree';
import { MatCheckboxModule } from '@angular/material/checkbox';

import { SectorNode, SectorsService } from '../../../services/sectors.service';

//interface SectorNode {
//  id: number;
//  name: string;
//  path: string;
//  isSelected: boolean;
//  children?: SectorNode[];
//}

/** Flat node with level information */
interface FlatSectorNode {
  name: string;
  level: number;
  id: number;
  isSelected: boolean;
  expandable: boolean;
}

@Component({
  selector: 'app-sectors-tree',
  standalone: true,
  imports: [
    CommonModule,
    MatTreeModule,
    MatCheckboxModule
  ],
  templateUrl: './sectors-tree.component.html',
  styleUrls: ['./sectors-tree.component.css']
})
export class SectorsTreeComponent implements OnInit {
  @Input() set sectorNodes(value: SectorNode[]) {
    this._sectorNodes = value;
    this.dataSource.data = this._sectorNodes;
    this.initializeSelection();
  }

  // Configuration inputs for hierarchical selection behavior
  @Input() selectChildrenWithParent = true;
  @Input() selectParentWithChildren = true;
  @Input() showIndeterminateState = true;

  @Output() selectedIdsChange = new EventEmitter<number[]>();

  private _sectorNodes: SectorNode[] = [];
  private _transformer = (node: SectorNode, level: number): FlatSectorNode => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      level: level,
      id: node.id,
      isSelected: node.isSelected
    };
  };

  treeControl = new FlatTreeControl<FlatSectorNode>(
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

  /** The selection for checkboxes */
  checklistSelection = new SelectionModel<FlatSectorNode>(true /* multiple */);

  constructor() {
    this.dataSource.data = [];
  }

  ngOnInit() {
    this.dataSource.data = this._sectorNodes;
    this.initializeSelection();
  }

  private initializeSelection() {
    // Pre-select nodes that have isSelected set to true
    setTimeout(() => {
      // Expand all nodes
      this.treeControl.expandAll();

      // Select nodes that are marked as selected
      const toSelect = this.treeControl.dataNodes.filter(node => node.isSelected);
      toSelect.forEach(node => this.checklistSelection.select(node));
      this.emitSelectedIds();
    });
  }

  hasChild = (_: number, node: FlatSectorNode) => node.expandable;

  /** Whether all descendants of the node are selected */
  descendantsAllSelected(node: FlatSectorNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    return descendants.length > 0 && descendants.every(child => this.checklistSelection.isSelected(child));
  }

  /** Whether part of the descendants are selected */
  descendantsPartiallySelected(node: FlatSectorNode): boolean {
    if (!this.showIndeterminateState) {
      return false;
    }

    const descendants = this.treeControl.getDescendants(node);
    const result = descendants.some(child => this.checklistSelection.isSelected(child));
    return result && !this.descendantsAllSelected(node);
  }

  /** Toggle the sector item selection. Select/deselect all the descendants node */
  sectorItemSelectionToggle(node: FlatSectorNode): void {
    this.checklistSelection.toggle(node);

    if (this.selectChildrenWithParent) {
      const descendants = this.treeControl.getDescendants(node);

      if (this.checklistSelection.isSelected(node)) {
        this.checklistSelection.select(...descendants);
      } else {
        this.checklistSelection.deselect(...descendants);
      }
    }

    if (this.selectParentWithChildren) {
      // Force update for the parent
      this.checkAllParentsSelection(node);
    }

    this.emitSelectedIds();
  }

  /** Toggle a leaf sector item selection */
  sectorLeafItemSelectionToggle(node: FlatSectorNode): void {
    this.checklistSelection.toggle(node);

    if (this.selectParentWithChildren) {
      this.checkAllParentsSelection(node);
    }

    this.emitSelectedIds();
  }

  /* Checks all the parents when a leaf node is selected/unselected */
  checkAllParentsSelection(node: FlatSectorNode): void {
    let parent: FlatSectorNode | null = this.getParentNode(node);
    while (parent) {
      this.checkRootNodeSelection(parent);
      parent = this.getParentNode(parent);
    }
  }

  /** Get the parent node of a node */
  getParentNode(node: FlatSectorNode): FlatSectorNode | null {
    const currentLevel = node.level;
    if (currentLevel < 1) {
      return null;
    }

    const startIndex = this.treeControl.dataNodes.indexOf(node) - 1;
    for (let i = startIndex; i >= 0; i--) {
      const currentNode = this.treeControl.dataNodes[i];
      if (currentNode.level < currentLevel) {
        return currentNode;
      }
    }
    return null;
  }

  /** Check root node checked state and change it accordingly */
  checkRootNodeSelection(node: FlatSectorNode): void {
    const nodeSelected = this.checklistSelection.isSelected(node);
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected = descendants.length > 0 && descendants.every(child => this.checklistSelection.isSelected(child));

    if (nodeSelected && !descAllSelected) {
      this.checklistSelection.deselect(node);
    } else if (!nodeSelected && descAllSelected) {
      this.checklistSelection.select(node);
    }
  }

  /** Get selected sector IDs and emit them */
  emitSelectedIds(): void {
    const selectedIds = this.checklistSelection.selected.map(node => node.id);
    this.selectedIdsChange.emit(selectedIds);
  }

  /** Get all currently selected node IDs */
  getSelectedIds(): number[] {
    return this.checklistSelection.selected.map(node => node.id);
  }
}
