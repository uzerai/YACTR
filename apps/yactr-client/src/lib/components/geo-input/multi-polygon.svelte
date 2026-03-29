<script lang="ts">
	import type { Coordinate } from 'ol/coordinate';
	import { Map, Layer, View, Interaction } from 'svelte-openlayers';
	import VectorSource from 'ol/source/Vector';
	import type { MultiPolygon } from '$lib/api';
	import { Button } from '$lib/components/ui/button';
	import { Polygon } from 'ol/geom';
	import { Feature, type View as OLView } from 'ol';
	import { untrack } from 'svelte';
	import type { ReactiveCollection } from 'svelte-openlayers/utils';
	import { m } from '$lib/paraglide/messages.js';
	import { fromEPSG3857ToSRID4326, fromSRID4326ToEPSG3857 } from '$lib/components/geo-input';

	let {
		boundary = $bindable(),
		mapCenter = [-74.006, 40.7128, 0],
		zoom = 12,
		disabled = false
	}: {
		boundary?: MultiPolygon | null;
		mapCenter?: Coordinate;
		zoom?: number;
		disabled?: boolean;
	} = $props();

	let vectorSource = $state(new VectorSource());
	let selectedFeatures: ReactiveCollection | null = $state(null);
	let view = $state<OLView | null>(null);

	$effect(() => {
		if (mapCenter) {
			view?.setCenter(fromSRID4326ToEPSG3857(mapCenter));
		}
	});
	
	untrack(() => {
		if (boundary && boundary.coordinates) {
			vectorSource.addFeatures(
				boundary.coordinates.map(polygon => new Feature({
					geometry: new Polygon(polygon
						.map(ring => ring
							.map(coordinate => fromSRID4326ToEPSG3857(coordinate))))
				}))
			)
		}
	});

	let isDrawing = $state(false);

	const onDrawEnd = ({ feature }: { feature: Feature<Polygon> }) => {
		isDrawing = false;
		
		if (feature.getGeometry() !== undefined) {
			boundary = {
				type: "MultiPolygon",
				coordinates: [
					...vectorSource.getFeatures().map(feature => (feature.getGeometry() as Polygon).getCoordinates()),
					feature.getGeometry()!.getCoordinates().map(coordinate => coordinate.map(fromEPSG3857ToSRID4326))
				]
			}
		}
	};

	const onModifyEnd = () => {
		boundary = {
				type: "MultiPolygon",
				coordinates: [
					...vectorSource.getFeatures()
						.map(feature => (feature.getGeometry() as Polygon)
							.getCoordinates().map(coordinate => coordinate
								.map(fromEPSG3857ToSRID4326)))
				]
			}
	};
</script>

<div class={`relative h-full w-full overflow-hidden rounded-lg border`}>
	{#if disabled}
		<div class="absolute top-0 right-0 z-20 h-full w-full cursor-not-allowed bg-gray-300 opacity-30"></div>
	{/if}
	<div class="absolute top-0 right-0 z-20 m-4 p-2 flex bg-gray-200/75 dark:bg-gray-700/50 rounded-lg">
		<div class="flex flex-col gap-1">
			<div class="flex gap-2">
				<Button variant="outline" onclick={() => isDrawing = true} disabled={isDrawing}>
					{m.geo_map_draw_polygon()}
				</Button>
				<Button variant="destructive" onclick={() => vectorSource.removeFeatures(selectedFeatures?.getArray()!)} disabled={isDrawing || (selectedFeatures?.getLength() ?? 0) < 1}>
					{m.geo_map_delete()}
				</Button>
			</div>
		</div>
	</div>
	<View bind:view {zoom}>
		<Map class="h-full w-full">
			<Layer.Tile source="osm" />

			<Layer.Vector bind:source={vectorSource}>
				{#if isDrawing}
					<Interaction.Draw minPoints={3} type="Polygon" bind:source={vectorSource} onDrawEnd={onDrawEnd} />
				{:else}
					<Interaction.Select bind:selectedFeatures />
					<Interaction.Modify bind:source={vectorSource} onModifyEnd={onModifyEnd} />
				{/if}
			</Layer.Vector>
		</Map>
	</View>
</div>
