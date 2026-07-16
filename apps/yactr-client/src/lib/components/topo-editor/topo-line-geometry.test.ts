import { describe, expect, it } from 'vitest';
import {
	distanceBetweenPoints,
	distancePointToSegment,
	findNearestSegment,
	isCloseToEndPoint
} from './topo-line-geometry';

describe('distanceBetweenPoints', () => {
	it('returns the euclidean distance', () => {
		expect(distanceBetweenPoints({ x: 0, y: 0 }, { x: 3, y: 4 })).toBe(5);
	});
});

describe('distancePointToSegment', () => {
	it('returns perpendicular distance when the projection falls inside the segment', () => {
		expect(distancePointToSegment({ x: 5, y: 3 }, { x: 0, y: 0 }, { x: 10, y: 0 })).toBe(3);
	});

	it('clamps to the nearest endpoint when the projection falls outside the segment', () => {
		expect(distancePointToSegment({ x: -3, y: 4 }, { x: 0, y: 0 }, { x: 10, y: 0 })).toBe(5);
		expect(distancePointToSegment({ x: 13, y: 4 }, { x: 0, y: 0 }, { x: 10, y: 0 })).toBe(5);
	});

	it('handles zero-length segments', () => {
		expect(distancePointToSegment({ x: 3, y: 4 }, { x: 0, y: 0 }, { x: 0, y: 0 })).toBe(5);
	});
});

describe('findNearestSegment', () => {
	const line = [
		{ x: 0, y: 0 },
		{ x: 10, y: 0 },
		{ x: 10, y: 10 }
	];

	it('returns undefined for fewer than two points', () => {
		expect(findNearestSegment([], { x: 0, y: 0 })).toBeUndefined();
		expect(findNearestSegment([{ x: 0, y: 0 }], { x: 0, y: 0 })).toBeUndefined();
	});

	it('finds the closest segment by start index', () => {
		expect(findNearestSegment(line, { x: 5, y: 1 })).toEqual({ index: 0, distance: 1 });
		expect(findNearestSegment(line, { x: 12, y: 5 })).toEqual({ index: 1, distance: 2 });
	});
});

describe('isCloseToEndPoint', () => {
	const line = [
		{ x: 0, y: 0 },
		{ x: 100, y: 0 }
	];

	it('returns false for an empty line', () => {
		expect(isCloseToEndPoint({ x: 0, y: 0 }, [])).toBe(false);
	});

	it('respects the threshold around the last point', () => {
		expect(isCloseToEndPoint({ x: 150, y: 0 }, line, 60)).toBe(true);
		expect(isCloseToEndPoint({ x: 200, y: 0 }, line, 60)).toBe(false);
	});
});
