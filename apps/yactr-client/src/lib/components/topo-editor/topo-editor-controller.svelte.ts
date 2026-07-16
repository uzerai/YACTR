import type { TopoEditorViewport } from './topo-editor-context.svelte';
import type { TopoEditorPoint } from './topo-editor-types';
import { distanceBetweenPoints, findNearestSegment, isCloseToEndPoint } from './topo-line-geometry';
import { TopoLineHistory } from './topo-line-history';
import { appendPoint, insertPoint, movePoint, removePoint } from './topo-line-ops';

export interface TopoEditorKeyedPoint extends TopoEditorPoint {
	id: string;
}

const createId = (): string =>
	typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function'
		? crypto.randomUUID()
		: Math.random().toString(36).slice(2);

const toKeyedPoint = (point: TopoEditorPoint): TopoEditorKeyedPoint => ({
	id: createId(),
	x: point.x,
	y: point.y
});

const haveSameCoordinates = (
	left: readonly TopoEditorPoint[],
	right: readonly TopoEditorPoint[]
): boolean =>
	left.length === right.length &&
	left.every((point, index) => point.x === right[index]?.x && point.y === right[index]?.y);

/**
 * Owns the topo line document (points with ephemeral editor-only ids), the
 * interaction state machine and the undo/redo history. The Konva overlay is a
 * thin view that forwards pointer/keyboard events here.
 *
 * Document state (undoable): the point list.
 * Ephemeral state (never undoable): drawing mode, hover preview, selection,
 * drag-in-progress.
 */
export class TopoEditorController {
	#viewport: TopoEditorViewport;
	#setPoints: (points: TopoEditorPoint[]) => void;
	#history = new TopoLineHistory<TopoEditorKeyedPoint>();
	/** Last plain array written out through the binding, to tell our own writes apart from external replacements. */
	#lastWrittenPoints: TopoEditorPoint[] | undefined;
	#pointsBeforeDrag: TopoEditorKeyedPoint[] | undefined;

	keyedPoints = $state<TopoEditorKeyedPoint[]>([]);
	isDrawing = $state(false);
	isDragging = $state(false);
	/** Start of the dashed preview segment, in natural image coordinates. */
	drawStartPoint = $state<TopoEditorPoint>();
	/** Pointer position for the preview segment, in canvas coordinates. */
	hoverPoint = $state<TopoEditorPoint>();
	selectedPointId = $state<string>();
	canUndo = $state(false);
	canRedo = $state(false);

	constructor(options: {
		viewport: TopoEditorViewport;
		setPoints: (points: TopoEditorPoint[]) => void;
	}) {
		this.#viewport = options.viewport;
		this.#setPoints = options.setPoints;
	}

	/**
	 * Sync from the bindable prop. A replacement that did not originate from
	 * this controller (form load/reset) regenerates ids and resets history.
	 */
	reconcileExternalPoints(external: TopoEditorPoint[] | null | undefined) {
		const points = external ?? [];
		if (points === this.#lastWrittenPoints) return;
		if (haveSameCoordinates(points, this.keyedPoints)) return;

		this.keyedPoints = points.map(toKeyedPoint);
		this.#lastWrittenPoints = points === external ? points : undefined;
		this.#history.clear();
		this.#syncHistoryFlags();
		this.selectedPointId = undefined;
		this.#pointsBeforeDrag = undefined;
		this.isDragging = false;
		this.cancelDrawing();
	}

	stagePointerDown(canvasPosition: TopoEditorPoint) {
		const rawPosition = this.#viewport.removeScaling(canvasPosition);

		if (!this.isDrawing && this.keyedPoints.length < 1) {
			this.#write(appendPoint(this.keyedPoints, toKeyedPoint(rawPosition)), 'commit');
			this.drawStartPoint = rawPosition;
			this.isDrawing = true;
			return;
		}

		if (!this.isDrawing) {
			if (this.selectedPointId) {
				this.selectedPointId = undefined;
				return;
			}

			if (this.keyedPoints.length >= 2 && isCloseToEndPoint(rawPosition, this.keyedPoints)) {
				this.resumeDrawingFromEnd();
			}
			return;
		}

		if (!this.drawStartPoint) return;

		const distance = distanceBetweenPoints(rawPosition, this.drawStartPoint);
		const stopThreshold = 8 / Math.min(this.#viewport.scaleX || 1, this.#viewport.scaleY || 1);
		if (distance <= stopThreshold) {
			this.cancelDrawing();
		} else {
			this.#write(appendPoint(this.keyedPoints, toKeyedPoint(rawPosition)), 'commit');
			this.drawStartPoint = rawPosition;
		}
	}

	pointerMove(canvasPosition: TopoEditorPoint) {
		this.hoverPoint = canvasPosition;
	}

	resumeDrawingFromEnd() {
		const end = this.keyedPoints[this.keyedPoints.length - 1];
		if (!end) return;

		this.selectedPointId = undefined;
		this.drawStartPoint = { x: end.x, y: end.y };
		this.isDrawing = true;
	}

	cancelDrawing() {
		this.isDrawing = false;
		this.drawStartPoint = undefined;
		this.hoverPoint = undefined;
	}

	selectPoint(pointId: string) {
		if (this.isDrawing) return;
		this.selectedPointId = pointId;
	}

	/** Returns true when a point was inserted on the nearest segment. */
	insertOnSegment(canvasPosition: TopoEditorPoint): boolean {
		if (this.isDrawing || this.keyedPoints.length < 2) return false;

		const rawPosition = this.#viewport.removeScaling(canvasPosition);
		const threshold = 12 / Math.min(this.#viewport.scaleX || 1, this.#viewport.scaleY || 1);
		const nearest = findNearestSegment(this.keyedPoints, rawPosition);

		if (!nearest || nearest.distance > threshold) return false;

		this.#write(insertPoint(this.keyedPoints, nearest.index + 1, toKeyedPoint(rawPosition)), 'commit');
		return true;
	}

	beginPointDrag() {
		this.isDragging = true;
		this.#pointsBeforeDrag = this.keyedPoints;
	}

	dragPointTo(pointId: string, canvasPosition: TopoEditorPoint) {
		const index = this.keyedPoints.findIndex((point) => point.id === pointId);
		if (index < 0) return;

		const rawPosition = this.#viewport.removeScaling(canvasPosition);
		this.#write(movePoint(this.keyedPoints, index, rawPosition), 'never');
	}

	endPointDrag(pointId: string, canvasPosition: TopoEditorPoint) {
		this.dragPointTo(pointId, canvasPosition);
		this.isDragging = false;

		const before = this.#pointsBeforeDrag;
		this.#pointsBeforeDrag = undefined;
		if (before && !haveSameCoordinates(before, this.keyedPoints)) {
			this.#history.commit(before);
			this.#syncHistoryFlags();
		}
	}

	removeSelectedPoint() {
		if (this.isDrawing || !this.selectedPointId) return;

		const index = this.keyedPoints.findIndex((point) => point.id === this.selectedPointId);
		if (index < 0) return;

		this.#write(removePoint(this.keyedPoints, index), 'commit');
		this.selectedPointId = undefined;
	}

	clearAll() {
		if (this.keyedPoints.length < 1) return;

		this.#write([], 'commit');
		this.selectedPointId = undefined;
		this.cancelDrawing();
	}

	undo() {
		this.cancelDrawing();
		const snapshot = this.#history.undo(this.keyedPoints);
		if (!snapshot) return;

		this.#restore(snapshot);
	}

	redo() {
		this.cancelDrawing();
		const snapshot = this.#history.redo(this.keyedPoints);
		if (!snapshot) return;

		this.#restore(snapshot);
	}

	escape() {
		if (this.isDrawing) {
			this.cancelDrawing();
		} else {
			this.selectedPointId = undefined;
		}
	}

	#restore(snapshot: TopoEditorKeyedPoint[]) {
		this.#write(snapshot, 'never');
		this.#syncHistoryFlags();
		if (
			this.selectedPointId &&
			!this.keyedPoints.some((point) => point.id === this.selectedPointId)
		) {
			this.selectedPointId = undefined;
		}
	}

	#write(next: TopoEditorKeyedPoint[], capture: 'commit' | 'never') {
		if (capture === 'commit') {
			this.#history.commit(this.keyedPoints);
			this.#syncHistoryFlags();
		}

		this.keyedPoints = next;
		const plain = next.map((point) => ({ x: point.x, y: point.y }));
		this.#lastWrittenPoints = plain;
		this.#setPoints(plain);
	}

	#syncHistoryFlags() {
		this.canUndo = this.#history.canUndo;
		this.canRedo = this.#history.canRedo;
	}
}
