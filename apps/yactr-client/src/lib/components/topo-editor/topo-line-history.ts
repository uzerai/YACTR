/**
 * Snapshot-based undo/redo stack. Each committed user action pushes the
 * previous document state; undo/redo swap snapshots between the two stacks.
 * Live updates (e.g. drag moves) are not committed — the interaction commits
 * once with the state captured at its start, coalescing the whole gesture
 * into a single undo step.
 */
export class TopoLineHistory<T> {
	#undoStack: T[][] = [];
	#redoStack: T[][] = [];

	get canUndo(): boolean {
		return this.#undoStack.length > 0;
	}

	get canRedo(): boolean {
		return this.#redoStack.length > 0;
	}

	/** Record the state as it was before a committed change. Clears redo. */
	commit(previous: readonly T[]): void {
		this.#undoStack.push([...previous]);
		this.#redoStack = [];
	}

	/** Returns the snapshot to restore, or undefined if nothing to undo. */
	undo(current: readonly T[]): T[] | undefined {
		const snapshot = this.#undoStack.pop();
		if (!snapshot) return undefined;

		this.#redoStack.push([...current]);
		return snapshot;
	}

	/** Returns the snapshot to restore, or undefined if nothing to redo. */
	redo(current: readonly T[]): T[] | undefined {
		const snapshot = this.#redoStack.pop();
		if (!snapshot) return undefined;

		this.#undoStack.push([...current]);
		return snapshot;
	}

	clear(): void {
		this.#undoStack = [];
		this.#redoStack = [];
	}
}
