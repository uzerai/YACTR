<script lang="ts">
	import type { HTMLAttributes } from 'svelte/elements';
	import { Feature } from 'ol';
	import type { Coordinate } from 'ol/coordinate';
	import { Point } from 'ol/geom';
	import type { Point as PointGeoJSON } from '$lib/api';
	import VectorSource from 'ol/source/Vector';
	import { untrack } from 'svelte';
	import { Map, Layer, View, Interaction } from 'svelte-openlayers';
	import { m } from '$lib/paraglide/messages.js';
	import type { View as OLView } from 'ol';

	let {
		location = $bindable(),
		mapCenter = [-74.006, 40.7128, 0],
		zoom = 12,
		disabled,
		...restProps
	}: HTMLAttributes<HTMLDivElement> & { location?: PointGeoJSON | null; mapCenter?: Coordinate; zoom?: number; disabled?: boolean; } = $props();

	let vectorSource = $state(new VectorSource());
	// required for dynamic map centering for OpenLayers - Svelte
	let view = $state<OLView | null>(null);

	$effect(() => {
		if (mapCenter) {
			view?.setCenter(mapCenter);
		}
	});

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

<div class="relative h-full w-full overflow-hidden rounded-lg border" {...restProps}>
	{#if disabled}
		<div
			class="absolute top-0 right-0 z-55 h-full w-full cursor-not-allowed bg-gray-300 opacity-30"
		></div>
	{/if}
	<div class="absolute top-0 right-0 z-20 m-4 flex">
		<p class="rounded-lg bg-gray-200/50 dark:bg-gray-700/50 p-2 text-sm">{m.geo_map_point_hint()}</p>
	</div>


	<View bind:view {zoom}>
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
