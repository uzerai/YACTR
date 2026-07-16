import { describe, expect, it } from 'vitest';
import { appendPoint, clearPoints, insertPoint, movePoint, removePoint } from './topo-line-ops';

const line = [
	{ id: 'a', x: 0, y: 0 },
	{ id: 'b', x: 10, y: 0 },
	{ id: 'c', x: 10, y: 10 }
];

describe('appendPoint', () => {
	it('adds the point at the end without mutating the input', () => {
		const result = appendPoint(line, { id: 'd', x: 20, y: 10 });

		expect(result.map((p) => p.id)).toEqual(['a', 'b', 'c', 'd']);
		expect(line).toHaveLength(3);
	});
});

describe('insertPoint', () => {
	it('inserts at the given index', () => {
		const result = insertPoint(line, 1, { id: 'x', x: 5, y: 0 });

		expect(result.map((p) => p.id)).toEqual(['a', 'x', 'b', 'c']);
		expect(line).toHaveLength(3);
	});
});

describe('movePoint', () => {
	it('replaces coordinates but keeps identity', () => {
		const result = movePoint(line, 1, { x: 42, y: 24 });

		expect(result[1]).toEqual({ id: 'b', x: 42, y: 24 });
		expect(result[0]).toBe(line[0]);
		expect(line[1]).toEqual({ id: 'b', x: 10, y: 0 });
	});
});

describe('removePoint', () => {
	it('removes the point at the given index', () => {
		const result = removePoint(line, 1);

		expect(result.map((p) => p.id)).toEqual(['a', 'c']);
		expect(line).toHaveLength(3);
	});
});

describe('clearPoints', () => {
	it('returns an empty array', () => {
		expect(clearPoints()).toEqual([]);
	});
});
