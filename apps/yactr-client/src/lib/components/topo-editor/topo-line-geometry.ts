import type { TopoEditorPoint } from './topo-editor-types';

export interface NearestSegment {
	/** Index of the segment's start point in the point list. */
	index: number;
	distance: number;
}

export const distanceBetweenPoints = (a: TopoEditorPoint, b: TopoEditorPoint): number =>
	Math.hypot(b.x - a.x, b.y - a.y);

export const distancePointToSegment = (
	point: TopoEditorPoint,
	start: TopoEditorPoint,
	end: TopoEditorPoint
): number => {
	const dx = end.x - start.x;
	const dy = end.y - start.y;
	const segmentLengthSquared = dx * dx + dy * dy;

	if (segmentLengthSquared === 0) {
		return Math.hypot(point.x - start.x, point.y - start.y);
	}

	const t = Math.max(
		0,
		Math.min(1, ((point.x - start.x) * dx + (point.y - start.y) * dy) / segmentLengthSquared)
	);
	const projectionX = start.x + t * dx;
	const projectionY = start.y + t * dy;
	return Math.hypot(point.x - projectionX, point.y - projectionY);
};

export const findNearestSegment = (
	points: readonly TopoEditorPoint[],
	position: TopoEditorPoint
): NearestSegment | undefined => {
	let nearest: NearestSegment | undefined;

	for (let i = 0; i < points.length - 1; i += 1) {
		const start = points[i];
		const end = points[i + 1];
		if (!start || !end) continue;

		const distance = distancePointToSegment(position, start, end);
		if (!nearest || distance < nearest.distance) {
			nearest = { index: i, distance };
		}
	}

	return nearest;
};

export const isCloseToEndPoint = (
	position: TopoEditorPoint,
	points: readonly TopoEditorPoint[],
	threshold: number = 120
): boolean => {
	const end = points[points.length - 1];
	if (!end) return false;

	return distanceBetweenPoints(position, end) <= threshold;
};
