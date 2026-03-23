<script lang="ts">
	import type { HTMLInputAttributes } from 'svelte/elements';
	import { P } from 'flowbite-svelte';
	import { Feature } from 'ol';
	import type { Coordinate } from 'ol/coordinate';
	import { Point } from 'ol/geom';
	import type { Point as PointGeoJSON } from '$lib/api';
	import VectorSource from 'ol/source/Vector';
	import { untrack } from 'svelte';
	import { Map, Layer, View, Interaction } from 'svelte-openlayers';

	let {
		location = $bindable(),
		mapCenter = [-74.006, 40.7128],
		disabled = false,
		...restProps
	}: HTMLInputAttributes & { location?: PointGeoJSON | null; mapCenter?: Coordinate; } = $props();

	let vectorSource = $state(new VectorSource());

	untrack(() => {
		if (location && location.coordinates) {
			vectorSource.addFeature(new Feature({
				geometry: new Point(location.coordinates)
			}));
		}
	});

	const onDrawEnd = ({ feature }: { feature: Feature<Point> }) => {
		vectorSource.clear();

		if (feature.getGeometry() !== undefined) {
			location = {
				type: "Point",
				coordinates: [
					...(feature.getGeometry()!.getCoordinates() as [number, number]),
					0
				]
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


	<View center={mapCenter} zoom={12}>
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
