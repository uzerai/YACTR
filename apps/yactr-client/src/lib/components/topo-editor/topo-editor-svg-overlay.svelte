<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { m } from '$lib/paraglide/messages.js';
	import {
		Circle,
		Layer,
		Line,
		Stage,
		type KonvaMouseEvent,
		type KonvaTouchEvent
	} from 'svelte-konva';
	import { getTopoEditorViewport } from './topo-editor-context.svelte';
	import { TopoEditorController } from './topo-editor-controller.svelte';
	import { buildTopoOverlayFile, buildTopoOverlaySignature } from './topo-editor-export';
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
	const controller = new TopoEditorController({
		viewport,
		setPoints: (next) => {
			points = next;
		}
	});

	// Sync external replacements (form load/reset) into the controller
	$effect(() => {
		controller.reconcileExternalPoints(points ?? []);
	});

	let overlaySource = $state<string>();
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
		if (controller.isDragging) {
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
		const svgOptions = {
			points: safePoints,
			naturalWidth: viewport.naturalWidth,
			naturalHeight: viewport.naturalHeight,
			strokeColor,
			strokeWidth: strokeWidth / safeScaleX
		};
		const signature = buildTopoOverlaySignature(svgOptions);

		if (signature === lastOutputSignature) {
			return;
		}

		lastOutputSignature = signature;
		output = buildTopoOverlayFile(svgOptions);
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
				strokeWidth: svgOptions.strokeWidth
			});
		}
	});

	const getPointerPosition = (e: KonvaMouseEvent | KonvaTouchEvent) =>
		e.target.getStage()?.getPointerPosition() ?? undefined;

	const handleStagePointerDown = (e: KonvaMouseEvent | KonvaTouchEvent) => {
		const pos = getPointerPosition(e);
		if (!pos) return;
		controller.stagePointerDown(pos);
	};

	const handleStagePointerMove = (e: KonvaMouseEvent | KonvaTouchEvent) => {
		const pos = getPointerPosition(e);
		if (!pos) return;
		controller.pointerMove(pos);
	};

	const handleLinePointerDown = (e: KonvaMouseEvent | KonvaTouchEvent) => {
		if (controller.isDrawing) return;

		// Avoid bubbling to Stage handlers (which can resume/start drawing)
		e.cancelBubble = true;

		const pos = getPointerPosition(e);
		if (!pos) return;
		controller.insertOnSegment(pos);
	};

	const handlePointPointerDown = (e: KonvaMouseEvent | KonvaTouchEvent, pointId: string, isLastPoint: boolean) => {
		if (controller.isDrawing) return; // bubble to stage to place the next vertex

		e.cancelBubble = true;

		if (isLastPoint && controller.keyedPoints.length >= 2) {
			controller.resumeDrawingFromEnd();
		} else {
			controller.selectPoint(pointId);
		}
	};

	const handleClear = () => {
		controller.clearAll();
	};

	const handleKeyDown = (event: KeyboardEvent) => {
		const isModifier = event.metaKey || event.ctrlKey;

		if (isModifier && event.key.toLowerCase() === 'z') {
			event.preventDefault();
			if (event.shiftKey) {
				controller.redo();
			} else {
				controller.undo();
			}
			return;
		}

		if (event.key === 'Delete' || event.key === 'Backspace') {
			if (controller.selectedPointId) {
				event.preventDefault();
				controller.removeSelectedPoint();
			}
			return;
		}

		if (event.key === 'Escape') {
			controller.escape();
		}
	};

	const setStageCursor = (e: KonvaMouseEvent | KonvaTouchEvent, cursor: string) => {
		e.target.getStage()?.container()?.style.setProperty('cursor', cursor);
	};
</script>

{#if overlaySource}
	<img
		alt={m.topo_editor_svg_overlay_image_alt()}
		class="pointer-events-none absolute top-0 right-0 bottom-0 left-0 z-10 select-none"
		src={overlaySource}
		width={viewport.canvasWidth}
		height={viewport.canvasHeight}
	/>
{/if}

{#if viewport.canvasWidth && viewport.canvasHeight}
	<!-- Focusable wrapper so the canvas editor can receive Delete/Undo/Redo keyboard shortcuts -->
	<!-- svelte-ignore a11y_no_noninteractive_tabindex, a11y_no_noninteractive_element_interactions -->
	<div
		role="application"
		aria-label={m.topo_editor_svg_overlay_image_alt()}
		tabindex="0"
		class="outline-none"
		onkeydown={handleKeyDown}
		onmousedown={(event) => event.currentTarget.focus()}
	>
		<Stage
			width={viewport.canvasWidth}
			height={viewport.canvasHeight}
			onmouseenter={(e) => {
				if (!controller.isDrawing && controller.keyedPoints.length < 1) {
					setStageCursor(e, 'crosshair');
				}
			}}
			onmousedown={handleStagePointerDown}
			onmousemove={handleStagePointerMove}
			onmouseleave={(e) => {
				setStageCursor(e, 'default');
				if (controller.isDrawing) {
					controller.cancelDrawing();
				}
			}}
			ontouchstart={handleStagePointerDown}
			ontouchmove={handleStagePointerMove}
		>
			<Layer>
				{#if controller.keyedPoints.length >= 2}
					<Line
						points={controller.keyedPoints.flatMap((point) => {
							const scaled = viewport.applyScaling(point);
							return [scaled.x, scaled.y];
						})}
						perfectDrawEnabled={false}
						stroke={strokeColor}
						strokeWidth={strokeWidth}
						lineCap="round"
						lineJoin="round"
						onmouseover={(e) => {
							if (!controller.isDrawing) {
								setStageCursor(e, 'copy');
							}
						}}
						onmouseleave={(e) => {
							setStageCursor(e, 'default');
						}}
						onmousedown={handleLinePointerDown}
						ontouchstart={handleLinePointerDown}
					/>
				{/if}

				{#if controller.isDrawing && controller.drawStartPoint && controller.hoverPoint}
					{@const scaledStartPoint = viewport.applyScaling(controller.drawStartPoint)}
					<Line
						points={[
							scaledStartPoint.x,
							scaledStartPoint.y,
							controller.hoverPoint.x,
							controller.hoverPoint.y
						]}
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

				{#if controller.isDrawing && controller.drawStartPoint}
					{@const scaledStartPoint = viewport.applyScaling(controller.drawStartPoint)}
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

				{#if !controller.isDrawing && controller.keyedPoints.length >= 2}
					{@const last = controller.keyedPoints[controller.keyedPoints.length - 1]}
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
				{#each controller.keyedPoints as point, index (point.id)}
					{@const isFirstPoint = index === 0}
					{@const isLastPoint = index === controller.keyedPoints.length - 1}
					{@const isConnectionPoint = !isFirstPoint && !isLastPoint}
					{@const isSelected = point.id === controller.selectedPointId}
					{@const scaledPoint = viewport.applyScaling(point)}
					<Circle
						x={scaledPoint.x}
						y={scaledPoint.y}
						radius={isSelected ? 6 : isConnectionPoint ? 4 : 3}
						fill={isConnectionPoint ? '#4ade80' : strokeColor}
						stroke={isSelected ? '#0ea5e9' : 'white'}
						strokeWidth={isSelected ? 3 : isConnectionPoint ? 2 : 1}
						draggable={!controller.isDrawing}
						perfectDrawEnabled={false}
						onmousedown={(e) => handlePointPointerDown(e, point.id, isLastPoint)}
						ontouchstart={(e) => handlePointPointerDown(e, point.id, isLastPoint)}
						ondragstart={() => {
							controller.beginPointDrag();
						}}
						onmouseover={(e) => {
							if (!controller.isDrawing && isLastPoint) {
								setStageCursor(e, 'copy');
							} else if (!controller.isDrawing) {
								setStageCursor(e, 'grab');
							}
						}}
						onmouseleave={(e) => {
							setStageCursor(e, 'default');
						}}
						ondragmove={(e) => {
							controller.dragPointTo(point.id, { x: e.target.x(), y: e.target.y() });
						}}
						ondragend={(e) => {
							controller.endPointDrag(point.id, { x: e.target.x(), y: e.target.y() });
						}}
					/>
				{/each}
			</Layer>
		</Stage>
	</div>
{/if}

<div class="absolute right-2 bottom-2 z-20 flex items-center gap-1 rounded-lg bg-accent">
	<Button
		variant="secondary"
		type="button"
		size="lg"
		onclick={() => controller.undo()}
		disabled={!controller.canUndo}
	>
		{m.topo_editor_svg_overlay_undo()}
	</Button>
	<Button
		variant="secondary"
		type="button"
		size="lg"
		onclick={() => controller.redo()}
		disabled={!controller.canRedo}
	>
		{m.topo_editor_svg_overlay_redo()}
	</Button>
	<Button
		variant="destructive"
		type="button"
		size="lg"
		onclick={handleClear}
		disabled={controller.keyedPoints.length < 1}
	>
		{m.topo_editor_svg_overlay_clear_topo()}
	</Button>
</div>
