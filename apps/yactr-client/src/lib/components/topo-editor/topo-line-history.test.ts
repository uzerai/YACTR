import { describe, expect, it } from 'vitest';
import { TopoLineHistory } from './topo-line-history';

type Point = { x: number; y: number };

const p = (x: number, y: number): Point => ({ x, y });

describe('TopoLineHistory', () => {
	it('starts empty', () => {
		const history = new TopoLineHistory<Point>();

		expect(history.canUndo).toBe(false);
		expect(history.canRedo).toBe(false);
		expect(history.undo([])).toBeUndefined();
		expect(history.redo([])).toBeUndefined();
	});

	it('undoes to the committed previous state and redoes forward again', () => {
		const history = new TopoLineHistory<Point>();
		const before = [p(0, 0)];
		const after = [p(0, 0), p(10, 10)];

		history.commit(before);
		expect(history.canUndo).toBe(true);

		const undone = history.undo(after);
		expect(undone).toEqual(before);
		expect(history.canUndo).toBe(false);
		expect(history.canRedo).toBe(true);

		const redone = history.redo(undone!);
		expect(redone).toEqual(after);
		expect(history.canUndo).toBe(true);
		expect(history.canRedo).toBe(false);
	});

	it('clears the redo stack on a new commit', () => {
		const history = new TopoLineHistory<Point>();

		history.commit([]);
		history.undo([p(1, 1)]);
		expect(history.canRedo).toBe(true);

		history.commit([p(2, 2)]);
		expect(history.canRedo).toBe(false);
	});

	it('copies snapshots so later mutations do not leak into history', () => {
		const history = new TopoLineHistory<Point>();
		const before = [p(0, 0)];

		history.commit(before);
		before.push(p(99, 99));

		expect(history.undo([])).toEqual([p(0, 0)]);
	});

	it('coalesces a drag into a single undo step when committed once at the end', () => {
		const history = new TopoLineHistory<Point>();
		const beforeDrag = [p(0, 0), p(10, 10)];

		// Intermediate drag positions are applied live without commits.
		const afterDrag = [p(0, 0), p(50, 50)];
		history.commit(beforeDrag);

		expect(history.undo(afterDrag)).toEqual(beforeDrag);
		expect(history.canUndo).toBe(false);
	});

	it('clear empties both stacks', () => {
		const history = new TopoLineHistory<Point>();

		history.commit([]);
		history.undo([p(1, 1)]);
		history.clear();

		expect(history.canUndo).toBe(false);
		expect(history.canRedo).toBe(false);
	});
});
