<script lang="ts">
	import type { Coordinate } from 'ol/coordinate';
	import { Map, Layer, View, Interaction } from 'svelte-openlayers';
	import VectorSource from 'ol/source/Vector';
	import type { Polygon as PolygonGeoJson } from '$lib/api';
	import { Button } from '$lib/components/ui/button';
	import { Polygon } from 'ol/geom';
	import { Feature } from 'ol';
	import { untrack } from 'svelte';
	import type { View as OLView } from 'ol';
	import { m } from '$lib/paraglide/messages.js';

	let {
		boundary = $bindable(),
		mapCenter = [-74.006, 40.7128, 0],
		zoom = 12,
		disabled = false
	}: {
		boundary?: PolygonGeoJson | null;
		mapCenter?: Coordinate;
		zoom?: number;
		disabled?: boolean;
	} = $props();

	let vectorSource = $state(new VectorSource());
	let view = $state<OLView | null>(null);

	$effect(() => {
		if (mapCenter) {
			view?.setCenter(mapCenter);
		}
	});

	untrack(() => {
		if (boundary && boundary.coordinates) {
			vectorSource.addFeature(
				new Feature({
					geometry: new Polygon(boundary.coordinates)
				})
			)
		}
	});

	let isDrawing = $state(false);

	const onDrawEnd = ({ feature }: { feature: Feature<Polygon> }) => {
    vectorSource.clear();
		isDrawing = false;
		
		if (feature.getGeometry() !== undefined) {
			boundary = {
				type: "Polygon",
				coordinates: feature.getGeometry()!.getCoordinates()
			}
		}
	};

	const onModifyEnd = () => {
    const singleFeature = vectorSource.getFeatures().at(0);

    if (singleFeature === undefined) return;

		boundary = {
      type: "Polygon",
      coordinates: (singleFeature.getGeometry() as Polygon)!.getCoordinates() as Coordinate[][]
    }
	};
</script>

<div class={`relative h-full w-full overflow-hidden rounded-lg border`}>
	{#if disabled}
		<div class="absolute top-0 right-0 z-20 h-full w-full cursor-not-allowed bg-gray-300 opacity-30"></div>
	{/if}
	<div class="absolute top-0 right-0 z-20 m-4 p-2 flex bg-gray-200/50 dark:bg-gray-700/50 rounded-lg">
		<div class="flex flex-col gap-1">
			<Button variant="outline" onclick={() => isDrawing = true} disabled={isDrawing}>
				{m.geo_map_draw_polygon()}
			</Button>
			<Button variant="destructive" onclick={() => vectorSource.clear()} disabled={isDrawing}>
				{m.geo_map_delete()}
			</Button>
		</div>
	</div>
	<View bind:view {zoom}>
		<Map class="h-full w-full">
			<Layer.Tile source="osm" />

			<Layer.Vector bind:source={vectorSource}>
				{#if isDrawing}
					<Interaction.Draw minPoints={3} type="Polygon" bind:source={vectorSource} onDrawEnd={onDrawEnd} />
				{:else}
					<Interaction.Modify bind:source={vectorSource} onModifyEnd={onModifyEnd} />
				{/if}
			</Layer.Vector>
		</Map>
	</View>
</div>
