<script lang="ts">
	import { Button, P } from 'flowbite-svelte';
	import {
		Layer,
		Stage,
		Line,
		Circle,
		type KonvaMouseEvent,
		type KonvaTouchEvent
	} from 'svelte-konva';
	import type { TopoEditorPoint } from '.';

	let {
		image = $bindable<string | undefined>(),
		points = $bindable<TopoEditorPoint[]>(),
		svg_file = $bindable<string | undefined>(),
		svg_output_file_upload = $bindable<HTMLInputElement>()
	} = $props();

	let canvas_height = $state<number>();
	let canvas_width = $state<number>();
	let background_image_element = $state<HTMLImageElement>();

	// Drawing state
	let is_drawing_line = $state(false);
	let line_start_point = $state<TopoEditorPoint>();
	let line_hover_point = $state<TopoEditorPoint>();
	let line_stroke_color = $state('#df4b26');
	let line_stroke_width = $state(5);

	// Responsive scaling
	let original_image_width = $state<number>();
	let original_image_height = $state<number>();
	let scale_x = $derived.by(() => (canvas_width ?? 1) / (original_image_width ?? 1));
	let scale_y = $derived.by(() => (canvas_height ?? 1) / (original_image_height ?? 1));

	const render_and_save = () => {
		const svg_string = `<?xml version="1.0" encoding="utf-8"?>
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${original_image_width} ${original_image_height}">
      <polyline points="${points?.map((point: TopoEditorPoint) => `${point.x},${point.y}`).join(' ')}" fill="none" stroke="red" stroke-width="${line_stroke_width / scale_x}" />
    </svg>`;

		const file = new File([svg_string], 'route_topo.svg', { type: 'image/svg+xml' });
		const dataTransfer = new DataTransfer();
		dataTransfer.items.add(file);

		if (svg_output_file_upload) {
			svg_output_file_upload.files = dataTransfer.files;
		}
		svg_file = URL.createObjectURL(file);
	};

	const is_close_to_end_point = (
		click_x: number,
		click_y: number,
		points: TopoEditorPoint[],
		threshold: number = 120
	) => {
		if (points.length < 1) return false;

		const end = points[points.length - 1];

		if (!end) return false;

		const distance = Math.sqrt(Math.pow(click_x - end.x, 2) + Math.pow(click_y - end.y, 2));
		return distance <= threshold;
	};

	const mouse_down = (e: KonvaMouseEvent | KonvaTouchEvent) => {
		const stage = e.target.getStage();
		const pos = stage?.getPointerPosition();

		if (!pos) return;

		const raw_position = remove_scaling(pos);

		// Handle clicking on the last point when not drawing (resumes drawing line)
		if (!is_drawing_line) {
			if (
				points &&
				points.length >= 2 &&
				is_close_to_end_point(raw_position.x, raw_position.y, points)
			) {
				const end = points[points.length - 1];
				if (end) {
					line_start_point = end;
					is_drawing_line = true;
				}
			}
		} else if (line_start_point) {
			const distance = Math.sqrt(
				Math.pow(raw_position.x - line_start_point.x, 2) +
					Math.pow(raw_position.y - line_start_point.y, 2)
			);
			if (distance <= 8 / Math.min(scale_x, scale_y)) {
				is_drawing_line = false;
				line_start_point = undefined;
				line_hover_point = undefined;
			} else {
				points = [...points, raw_position];
				line_start_point = raw_position;
			}
		} else if (!points || points.length < 1) {
			// Start a completely new line
			line_start_point = raw_position;
			points = [line_start_point];
		}
	};

	const mouse_move = (e: KonvaMouseEvent | KonvaTouchEvent) => {
		const stage = e.target.getStage();
		const point = stage!.getPointerPosition()!;

		line_hover_point = { x: point.x, y: point.y };
	};

	const clear_points = () => {
		points = [];
		is_drawing_line = false;
		line_start_point = undefined;
		line_hover_point = undefined;
	};

	const point_drag = (point_index: number, new_x: number, new_y: number) => {
		points[point_index] = { x: new_x, y: new_y };
	};

	const apply_scaling = (point: TopoEditorPoint) => {
		return {
			x: point.x * scale_x,
			y: point.y * scale_y
		} as TopoEditorPoint;
	};

	const remove_scaling = (point: TopoEditorPoint) => {
		return {
			x: point.x / scale_x,
			y: point.y / scale_y
		} as TopoEditorPoint;
	};
</script>

<div class="flex flex-col gap-2">
	{#if image}
		<div class="relative">
			<img
				alt={'Background for route drawing'}
				src={image}
				class="object-fit pointer-events-none absolute top-0 right-0 bottom-0 left-0 select-none"
				bind:this={background_image_element}
				bind:clientWidth={canvas_width}
				bind:clientHeight={canvas_height}
				onload={(event) => {
					if (event.currentTarget) {
						console.log('setting original width/height');
						const element = event.currentTarget as HTMLImageElement;
						original_image_width = element.naturalWidth;
						original_image_height = element.naturalHeight;
					}
				}}
			/>

			{#if svg_file}
				<img
					alt="Route drawing svg, indicating the route itself"
					class="pointer-events-none absolute top-0 right-0 bottom-0 left-0 z-10 select-none"
					src={svg_file}
					width={canvas_width}
					height={canvas_height}
				/>
			{/if}

			{#if background_image_element && canvas_width && canvas_height}
				<Stage
					width={canvas_width}
					height={canvas_height}
					onmousedown={mouse_down}
					onmousemove={mouse_move}
					onmouseleave={() => {
						if (is_drawing_line) {
							is_drawing_line = false;
							line_start_point = undefined;
							line_hover_point = undefined;
						}
					}}
					ontouchstart={mouse_down}
					ontouchmove={mouse_move}
				>
					<Layer>
						<!-- Drawn Line -->
						{#if points && points.length >= 2}
							<Line
								points={points?.flatMap((point: TopoEditorPoint) => {
									const scaled = apply_scaling(point);
									return [scaled.x, scaled.y];
								})}
								stroke={line_stroke_color}
								strokeWidth={line_stroke_width}
								lineCap="round"
								lineJoin="round"
							/>
						{/if}

						<!-- Preview Line (shows while hovering) -->
						{#if is_drawing_line && line_start_point && line_hover_point}
							{@const scaled_start_point = apply_scaling(line_start_point)}
							<Line
								points={[
									scaled_start_point.x,
									scaled_start_point.y,
									line_hover_point.x,
									line_hover_point.y
								]}
								stroke={line_stroke_color}
								strokeWidth={line_stroke_width}
								lineCap="round"
								lineJoin="round"
								dash={[5, 5]}
								opacity={0.7}
							/>
						{/if}

						<!-- Start Point Indicator (when actively drawing) -->
						{#if is_drawing_line && line_start_point}
							{@const scaled_start_point = apply_scaling(line_start_point)}
							<Circle
								x={scaled_start_point.x}
								y={scaled_start_point.y}
								radius={4}
								fill={line_stroke_color}
								stroke="white"
								strokeWidth={2}
							/>
						{/if}

						<!-- Resume Point Indicator (when drawing is paused) -->
						{#if !is_drawing_line && points && points.length >= 2}
							{@const last = points[points.length - 1]}
							{#if last}
								{@const scaledLast = apply_scaling(last)}
								<Circle
									x={scaledLast.x}
									y={scaledLast.y}
									radius={6}
									fill="#4ade80"
									stroke="white"
									strokeWidth={3}
									opacity={0.8}
								/>
								<!-- Inner circle to show it's clickable -->
								<Circle x={scaledLast.x} y={scaledLast.y} radius={3} fill="white" opacity={0.9} />
							{/if}
						{/if}
					</Layer>
					<!-- Point layer, always shows above the line -->
					<Layer>
						<!-- Line Points -->
						{#each points as point, index}
							{@const is_first_point = index === 0}
							{@const is_last_point = index === points.length - 1}
							{@const is_connection_point = !is_first_point && !is_last_point}
							{@const scaled_point = apply_scaling(point)}
							<Circle
								x={scaled_point.x}
								y={scaled_point.y}
								radius={is_connection_point ? 4 : 3}
								fill={is_connection_point ? '#4ade80' : line_stroke_color}
								stroke="white"
								strokeWidth={is_connection_point ? 2 : 1}
								draggable={!is_drawing_line}
								onmousedown={() => {
									if (is_last_point && is_drawing_line) {
										is_drawing_line = false;
									}
								}}
								onmouseover={(e) => {
									if (!is_drawing_line && is_last_point) {
										e.target.getStage()!.container()!.style.cursor = 'copy';
									} else if (!is_drawing_line) {
										e.target.getStage()!.container()!.style.cursor = 'grab';
									}
								}}
								onmouseleave={(e) => {
									e.target.getStage()!.container()!.style.cursor = 'default';
								}}
								ondragend={(e) => {
									if (!is_drawing_line) {
										const unscaledPos = remove_scaling({ x: e.target.x(), y: e.target.y() });
										point_drag(index, unscaledPos.x, unscaledPos.y);
									}
								}}
							/>
						{/each}
					</Layer>
				</Stage>
			{/if}
		</div>
	{/if}
	<div class="flex gap-2">
		{#if (points && points.length > 0) || is_drawing_line}
			<Button color="red" type="button" size="sm" onclick={clear_points}>Clear topo</Button>
		{:else}
			<Button
				color="blue"
				type="button"
				size="sm"
				onclick={() => (is_drawing_line = true)}
				disabled={!image}>Draw topo</Button
			>
		{/if}
		{#if !svg_file}
			<Button
				type="button"
				size="sm"
				color="green"
				onclick={render_and_save}
				disabled={(points?.length ?? 0) < 2}>Save and Render</Button
			>
		{:else}
			<Button
				type="button"
				size="sm"
				color="gray"
				onclick={() => {
					if (svg_output_file_upload) {
						svg_output_file_upload.files = new DataTransfer().files;
					}
					svg_file = undefined;
				}}>Clear current Topo</Button
			>
		{/if}
	</div>
	<P size="xs">Dot indicators on route line will not be rendered in final output.</P>
</div>
