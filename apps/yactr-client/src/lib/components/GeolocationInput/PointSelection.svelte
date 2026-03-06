<script lang="ts">
	import type { YactrApiEndpointsAreasAreaRequestData } from '$lib/api';
	import { P } from 'flowbite-svelte';
	import type { Feature } from 'ol';
	import type { Coordinate } from 'ol/coordinate';
	import type { Point, PointFeature } from 'ol/renderer/webgl/PointsLayer';
	import VectorSource from 'ol/source/Vector';
	import { Map, Layer, View, Interaction } from 'svelte-openlayers';

	let {
		location = $bindable(),
		mapCenter: center = $bindable([-74.006, 40.7128]),
		disabled = false
	}: { location?: YactrApiEndpointsAreasAreaRequestData['location']; mapCenter?: Coordinate; disabled?: boolean } = $props();

	let zoom = $state(12);
	let vectorSource = $state(new VectorSource());

	const onDrawEnd = ({ feature }: { feature: Feature<Point> }) => {
		vectorSource.clear();

		if (feature.getGeometry() !== undefined) {
			location = {
				type: "Point",
				coordinates: (feature.getGeometry()?.getCoordinates() ?? []) as [number, number, number]
			};
		}
	};
</script>

<div class="relative h-full w-full overflow-hidden rounded-lg border">
	{#if disabled}
		<div
			class="absolute top-0 right-0 z-55 h-full w-full cursor-not-allowed bg-gray-300 opacity-30"
		></div>
	{/if}
	<div class="absolute top-0 right-0 z-20 m-4 flex">
		<P class="rounded-lg bg-gray-200/50 dark:bg-gray-700/50 p-2 text-sm">Click anywhere on the map to place the point.</P>
	</div>


	<View bind:center bind:zoom>
		<Map class="h-full w-full">
			<Layer.Tile source="osm" />

			<Layer.Vector bind:source={vectorSource}>
				<Interaction.Draw type="Point" bind:source={vectorSource}
					maxPoints={1}
					onDrawEnd={onDrawEnd} />
			</Layer.Vector>
		</Map>
	</View>
</div>
