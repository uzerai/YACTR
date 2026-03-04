<script lang="ts">
	import { P } from 'flowbite-svelte';
	import type { Coordinate } from 'ol/coordinate';
	import { Map, Layer, Feature } from 'svelte-openlayers';
	import { createCircleStyle } from 'svelte-openlayers/utils';

	let {
		location = $bindable(),
		mapCenter: center = $bindable([-74.006, 40.7128]),
		disabled = false
	}: { location?: Coordinate; mapCenter?: Coordinate; disabled?: boolean } = $props();

	let zoom = $state(12);

	const pointStyle = createCircleStyle({
		radius: 10,
		fill: '#4338ca', // Uses --ol-color-primary
		stroke: '#ffffff',
		strokeWidth: 2
	});
</script>

<div class="relative h-full w-full overflow-hidden rounded-lg border">
	{#if disabled}
		<div
			class="absolute top-0 right-0 z-55 h-full w-full cursor-not-allowed bg-gray-300 opacity-30"
		></div>
	{/if}
	<div class="absolute top-0 right-0 z-20 m-4 flex">
		<P class="rounded-lg bg-gray-200 p-4">Click anywhere on the map to place the point.</P>
	</div>
	<Map.Root
		onClick={({ coordinate }) => {
			location = coordinate;
		}}
	>
		<Map.View bind:center bind:zoom />
		<Layer.Tile source="osm" />
		<Layer.Vector style={pointStyle}>
			{#if location}
				<Feature.Point coordinates={location} />
			{/if}
		</Layer.Vector>
	</Map.Root>
</div>
