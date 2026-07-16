import type { TopoEditorPoint } from './topo-editor-types';

/**
 * Pure operations on point lists. All functions return a new array and never
 * mutate the input, so snapshots held by the history stay valid.
 */

export const appendPoint = <T extends TopoEditorPoint>(points: readonly T[], point: T): T[] => [
	...points,
	point
];

export const insertPoint = <T extends TopoEditorPoint>(
	points: readonly T[],
	index: number,
	point: T
): T[] => {
	const updated = [...points];
	updated.splice(index, 0, point);
	return updated;
};

export const movePoint = <T extends TopoEditorPoint>(
	points: readonly T[],
	index: number,
	position: TopoEditorPoint
): T[] =>
	points.map((point, i) => (i === index ? { ...point, x: position.x, y: position.y } : point));

export const removePoint = <T extends TopoEditorPoint>(points: readonly T[], index: number): T[] =>
	points.filter((_, i) => i !== index);

export const clearPoints = <T extends TopoEditorPoint>(): T[] => [];
