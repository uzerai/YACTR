<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import {
		Circle,
		Layer,
		Line,
		Stage,
		type KonvaMouseEvent,
		type KonvaTouchEvent
	} from 'svelte-konva';
	import { getTopoEditorViewport } from './topo-editor-context.svelte';
	import type { TopoEditorPoint } from './topo-editor-types';

	interface TopoEditorLineStyle {
		stroke?: string;
		strokeWidth?: number;
		previewDash?: number[];
	}

	let {
		source = $bindable<File | string | undefined>(),
		output = $bindable<File | undefined>(),
		points = $bindable<TopoEditorPoint[] | null | undefined>([]),
		debug = false,
		lineStyle = {}
	}: {
		source?: File | string | undefined;
		output?: File | undefined;
		points?: TopoEditorPoint[] | null | undefined;
		debug?: boolean;
		lineStyle?: TopoEditorLineStyle;
	} = $props();

	// Normalize nullable points to an empty array for internal usage
	let safePoints = $derived<TopoEditorPoint[]>(points ?? []);

	const viewport = getTopoEditorViewport();

	let overlaySource = $state<string>();
	let isDrawingLine = $state(false);
	let isDraggingPoint = $state(false);
	let lineStartPoint = $state<TopoEditorPoint>();
	let lineHoverPoint = $state<TopoEditorPoint>();
	let lastOutputSignature = $state<string>();

	let strokeColor = $derived(lineStyle.stroke ?? '#df4b26');
	let strokeWidth = $derived(lineStyle.strokeWidth ?? 5);
	let previewDash = $derived(lineStyle.previewDash ?? [5, 5]);

	$effect(() => {
		if (!source) {
			overlaySource = undefined;
			return;
		}

		if (typeof source === 'string') {
			overlaySource = source;
			return;
		}

		const objectUrl = URL.createObjectURL(source);
		overlaySource = objectUrl;

		return () => {
			URL.revokeObjectURL(objectUrl);
		};
	});

	$effect(() => {
		if (isDraggingPoint) {
			return;
		}

		if (safePoints.length < 2 || !viewport.naturalWidth || !viewport.naturalHeight) {
			if (lastOutputSignature !== undefined) {
				lastOutputSignature = undefined;
				output = undefined;
				if (debug) {
					console.debug('[TopoEditor.SvgOverlay] output cleared');
				}
			}
			return;
		}

		const safeScaleX = viewport.scaleX || 1;
		const normalizedStroke = strokeWidth / safeScaleX;
		const pointsSignature = safePoints.map((point) => `${point.x},${point.y}`).join(' ');
		const signature = `${viewport.naturalWidth}x${viewport.naturalHeight}|${normalizedStroke}|${pointsSignature}`;

		if (signature === lastOutputSignature) {
			return;
		}

		const svgString = `<?xml version="1.0" encoding="utf-8"?>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${viewport.naturalWidth} ${viewport.naturalHeight}">
	<polyline points="${pointsSignature}" fill="none" stroke="${strokeColor}" stroke-width="${normalizedStroke}" />
</svg>`;

		lastOutputSignature = signature;
		output = new File([svgString], 'route_topo.svg', { type: 'image/svg+xml' });
		if (debug) {
			console.debug('[TopoEditor.SvgOverlay] output committed', {
				filename: output.name,
				size: output.size,
				pointCount: safePoints.length,
				naturalSize: {
					width: viewport.naturalWidth,
					height: viewport.naturalHeight
				},
				strokeColor,
				strokeWidth: normalizedStroke
			});
		}
	});

	const isCloseToEndPoint = (
		clickX: number,
		clickY: number,
		candidatePoints: TopoEditorPoint[],
		threshold: number = 120
	) => {
		if (candidatePoints.length < 1) return false;
		const end = candidatePoints[candidatePoints.length - 1];
		if (!end) return false;

		const distance = Math.sqrt(Math.pow(clickX - end.x, 2) + Math.pow(clickY - end.y, 2));
		return distance <= threshold;
	};

	const mouseDown = (e: KonvaMouseEvent | KonvaTouchEvent) => {
		const stage = e.target.getStage();
		const pos = stage?.getPointerPosition();

		if (!pos) return;

		const rawPosition = viewport.removeScaling({ x: pos.x, y: pos.y });

		if (!isDrawingLine && safePoints.length < 1) {
			points = [rawPosition];
			lineStartPoint = rawPosition;
			isDrawingLine = true;
			return;
		}

		if (!isDrawingLine) {
			if (safePoints.length >= 2 && isCloseToEndPoint(rawPosition.x, rawPosition.y, safePoints)) {
				const end = safePoints[safePoints.length - 1];
				if (end) {
					lineStartPoint = end;
					isDrawingLine = true;
				}
			}
			return;
		}

		if (lineStartPoint) {
			const distance = Math.sqrt(
				Math.pow(rawPosition.x - lineStartPoint.x, 2) + Math.pow(rawPosition.y - lineStartPoint.y, 2)
			);
			if (distance <= 8 / Math.min(viewport.scaleX || 1, viewport.scaleY || 1)) {
				isDrawingLine = false;
				lineStartPoint = undefined;
				lineHoverPoint = undefined;
			} else {
				points = [...safePoints, rawPosition];
				lineStartPoint = rawPosition;
			}
		}
	};

	const mouseMove = (e: KonvaMouseEvent | KonvaTouchEvent) => {
		const stage = e.target.getStage();
		const point = stage?.getPointerPosition();
		if (!point) return;
		lineHoverPoint = { x: point.x, y: point.y };
	};

	const clearPoints = () => {
		points = [];
		lastOutputSignature = undefined;
		output = undefined;
		isDrawingLine = false;
		lineStartPoint = undefined;
		lineHoverPoint = undefined;
	};

	const pointDrag = (pointIndex: number, newX: number, newY: number) => {
		const updated = [...safePoints];
		updated[pointIndex] = { x: newX, y: newY };
		points = updated;
	};

	const distancePointToSegment = (point: TopoEditorPoint, start: TopoEditorPoint, end: TopoEditorPoint) => {
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

	const insertPointOnLine = (e: KonvaMouseEvent | KonvaTouchEvent) => {
		if (isDrawingLine || safePoints.length < 2) return;

		const stage = e.target.getStage();
		const pos = stage?.getPointerPosition();
		if (!pos) return;

		// Avoid bubbling to Stage handlers (which can resume/start drawing)
		e.cancelBubble = true;

		const rawPosition = viewport.removeScaling({ x: pos.x, y: pos.y });
		const rawThreshold = 12 / Math.min(viewport.scaleX || 1, viewport.scaleY || 1);

		let nearestSegmentIndex = -1;
		let nearestDistance = Number.POSITIVE_INFINITY;

		for (let i = 0; i < safePoints.length - 1; i += 1) {
			const start = safePoints[i];
			const end = safePoints[i + 1];
			if (!start || !end) continue;

			const distance = distancePointToSegment(rawPosition, start, end);
			if (distance < nearestDistance) {
				nearestDistance = distance;
				nearestSegmentIndex = i;
			}
		}

		if (nearestSegmentIndex < 0 || nearestDistance > rawThreshold) {
			return;
		}

		const updated = [...safePoints];
		updated.splice(nearestSegmentIndex + 1, 0, rawPosition);
		points = updated;
	};
</script>

{#if overlaySource}
	<img
		alt="Route drawing svg, indicating the route itself"
		class="pointer-events-none absolute top-0 right-0 bottom-0 left-0 z-10 select-none"
		src={overlaySource}
		width={viewport.canvasWidth}
		height={viewport.canvasHeight}
	/>
{/if}

{#if viewport.canvasWidth && viewport.canvasHeight}
	<Stage
		width={viewport.canvasWidth}
		height={viewport.canvasHeight}
		onmousedown={mouseDown}
		onmousemove={mouseMove}
		onmouseleave={() => {
			if (isDrawingLine) {
				isDrawingLine = false;
				lineStartPoint = undefined;
				lineHoverPoint = undefined;
			}
		}}
		ontouchstart={mouseDown}
		ontouchmove={mouseMove}
	>
		<Layer>
			{#if safePoints.length >= 2}
				<Line
					points={safePoints.flatMap((point) => {
						const scaled = viewport.applyScaling(point);
						return [scaled.x, scaled.y];
					})}
					perfectDrawEnabled={false}
					stroke={strokeColor}
					strokeWidth={strokeWidth}
					lineCap="round"
					lineJoin="round"
					onmouseover={(e) => {
						if (!isDrawingLine) {
							e.target.getStage()?.container()?.style.setProperty('cursor', 'copy');
						}
					}}
					onmouseleave={(e) => {
						e.target.getStage()?.container()?.style.setProperty('cursor', 'default');
					}}
					onmousedown={insertPointOnLine}
					ontouchstart={insertPointOnLine}
				/>
			{/if}

			{#if isDrawingLine && lineStartPoint && lineHoverPoint}
				{@const scaledStartPoint = viewport.applyScaling(lineStartPoint)}
				<Line
					points={[scaledStartPoint.x, scaledStartPoint.y, lineHoverPoint.x, lineHoverPoint.y]}
					listening={false}
					perfectDrawEnabled={false}
					stroke={strokeColor}
					strokeWidth={strokeWidth}
					lineCap="round"
					lineJoin="round"
					dash={previewDash}
					opacity={0.7}
				/>
			{/if}

			{#if isDrawingLine && lineStartPoint}
				{@const scaledStartPoint = viewport.applyScaling(lineStartPoint)}
				<Circle
					x={scaledStartPoint.x}
					y={scaledStartPoint.y}
					listening={false}
					perfectDrawEnabled={false}
					radius={4}
					fill={strokeColor}
					stroke="white"
					strokeWidth={2}
				/>
			{/if}

			{#if !isDrawingLine && safePoints.length >= 2}
				{@const last = safePoints[safePoints.length - 1]}
				{#if last}
					{@const scaledLast = viewport.applyScaling(last)}
					<Circle
						x={scaledLast.x}
						y={scaledLast.y}
						listening={false}
						perfectDrawEnabled={false}
						radius={6}
						fill="#4ade80"
						stroke="white"
						strokeWidth={3}
						opacity={0.8}
					/>
					<Circle
						x={scaledLast.x}
						y={scaledLast.y}
						listening={false}
						perfectDrawEnabled={false}
						radius={3}
						fill="white"
						opacity={0.9}
					/>
				{/if}
			{/if}
		</Layer>

		<Layer>
			{#each safePoints as point, index}
				{@const isFirstPoint = index === 0}
				{@const isLastPoint = index === safePoints.length - 1}
				{@const isConnectionPoint = !isFirstPoint && !isLastPoint}
				{@const scaledPoint = viewport.applyScaling(point)}
				<Circle
					x={scaledPoint.x}
					y={scaledPoint.y}
					radius={isConnectionPoint ? 4 : 3}
					fill={isConnectionPoint ? '#4ade80' : strokeColor}
					stroke="white"
					strokeWidth={isConnectionPoint ? 2 : 1}
					draggable={!isDrawingLine}
					perfectDrawEnabled={false}
					ondragstart={() => {
						isDraggingPoint = true;
					}}
					onmouseover={(e) => {
						if (!isDrawingLine && isLastPoint) {
							e.target.getStage()?.container()?.style.setProperty('cursor', 'copy');
						} else if (!isDrawingLine) {
							e.target.getStage()?.container()?.style.setProperty('cursor', 'grab');
						}
					}}
					onmouseleave={(e) => {
						e.target.getStage()?.container()?.style.setProperty('cursor', 'default');
					}}
					ondragmove={(e) => {
						if (!isDrawingLine) {
							const unscaledPos = viewport.removeScaling({ x: e.target.x(), y: e.target.y() });
							pointDrag(index, unscaledPos.x, unscaledPos.y);
						}
					}}
					ondragend={(e) => {
						if (!isDrawingLine) {
							const unscaledPos = viewport.removeScaling({ x: e.target.x(), y: e.target.y() });
							pointDrag(index, unscaledPos.x, unscaledPos.y);
						}
						isDraggingPoint = false;
					}}
				/>
			{/each}
		</Layer>
	</Stage>
{/if}

<div class="absolute right-2 bottom-2 z-20">
	<Button variant="destructive" type="button" size="sm" onclick={clearPoints} disabled={safePoints.length < 1}>
		Clear topo
	</Button>
</div>
