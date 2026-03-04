<script lang="ts">
	import { Button } from 'flowbite-svelte';
	import type { Coordinate } from 'ol/coordinate';
	import type { Feature as OlFeature } from 'ol';
	import { Map, Layer, Feature, Interaction } from 'svelte-openlayers';
	import { createCircleStyle, createStyle, ReactiveCollection } from 'svelte-openlayers/utils';

	let {
		line = $bindable(),
		mapCenter: center = $bindable([-74.006, 40.7128]),
		zoom = $bindable(12),
		disabled = false
	}: {
		line: Coordinate[];
		mapCenter?: Coordinate;
		zoom?: number;
		disabled?: boolean;
	} = $props();

	let drawingMode = $state(false);
	let selectedFeatures: ReactiveCollection<OlFeature> = $state(null);

	const pointStyle = createCircleStyle({
		radius: 10,
		fill: '#4338ca', // Uses --ol-color-primary
		stroke: '#ffffff',
		strokeWidth: 2
	});

	const lineStyle = createStyle({
		stroke: {
			color: '#4338ca',
			width: 2
		}
	});

	const mapClick = ({ coordinate }: { coordinate: Coordinate }) => {
		if (drawingMode) {
			line = [...line, coordinate];
		}
	};
</script>

<div class="relative h-full w-full overflow-hidden rounded-lg border">
	{#if disabled}
		<div
			class="absolute top-0 right-0 z-20 h-full w-full cursor-not-allowed bg-gray-300 opacity-30"
		></div>
	{:else}
		<div class="absolute top-0 right-0 z-20 m-4 flex gap-1 text-xs">
			{#if !drawingMode}
				<Button color="green" onclick={() => (drawingMode = true)} disabled={line.length > 0}
					>Add path</Button
				>
			{:else}
				<Button color="blue" onclick={() => (drawingMode = false)}>Save</Button>
			{/if}

			<Button color="red" disabled={line.length < 1} onclick={() => (line = [])}>Delete</Button>
		</div>
	{/if}
	<Map.Root onClick={mapClick}>
		<Map.View bind:center bind:zoom />
		<Layer.Tile source="osm" />
		<Layer.Vector style={pointStyle}>
			{#each line as coordinate}
				<Feature.Point coordinates={coordinate} />
			{/each}
		</Layer.Vector>
		<Layer.Vector style={lineStyle}>
			{#if line.length > 0}
				<Feature.LineString coordinates={line} properties={{ type: 'lineString' }} />
			{/if}
		</Layer.Vector>

		<Interaction.Select
			filter={(feature: OlFeature) => feature.getProperties().type === 'lineString'}
			bind:selectedFeatures
			multi={false}
		/>
	</Map.Root>
</div>
