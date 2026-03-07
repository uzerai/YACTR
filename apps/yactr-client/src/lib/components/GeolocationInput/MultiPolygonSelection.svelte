<script lang="ts">
	import type { Coordinate } from 'ol/coordinate';
	import { Map, Layer, View, Interaction, FeaturePolygon } from 'svelte-openlayers';
	import VectorSource from 'ol/source/Vector';
	import type { NetTopologySuiteGeometriesMultiPolygon, YactrApiEndpointsAreasAreaRequestData } from '$lib/api';
	import { Button, P } from 'flowbite-svelte';
	import { Polygon } from 'ol/geom';
	import { Feature } from 'ol';

	let {
		boundary = $bindable(),
		mapCenter: center = $bindable([-74.006, 40.7128]),
		zoom = $bindable(12),
		disabled = false
	}: {
		boundary?: NetTopologySuiteGeometriesMultiPolygon;
		mapCenter?: Coordinate;
		zoom?: number;
		disabled?: boolean;
	} = $props();

	let vectorSource = $state(new VectorSource());
	
	if (boundary && boundary.coordinates) {
		vectorSource.addFeatures(
			boundary.coordinates.map(polygon => new Feature({
				geometry: new Polygon(polygon)
			}))
		)
	}

	let isDrawing = $state(false);

	const onDrawEnd = ({ feature }: { feature: Feature<Polygon> }) => {
		isDrawing = false;
		
		if (feature.getGeometry() !== undefined) {
			boundary = {
				type: "MultiPolygon",
				coordinates: [
					...vectorSource.getFeatures().map(feature => (feature.getGeometry() as Polygon).getCoordinates() as Coordinate[][]),
					feature.getGeometry()!.getCoordinates() as Coordinate[][]
				]
			}
		}
	};

	const onModifyEnd = () => {
		boundary = {
				type: "MultiPolygon",
				coordinates: [
					...vectorSource.getFeatures().map(feature => (feature.getGeometry() as Polygon).getCoordinates() as Coordinate[][])
				]
			}
	};
</script>

<div class={`relative h-full w-full overflow-hidden rounded-lg border`}>
	{#if disabled}
		<div class="absolute top-0 right-0 z-20 h-full w-full cursor-not-allowed bg-gray-300 opacity-30"></div>
	{/if}
	<div class="absolute top-0 right-0 z-20 m-4 p-2 flex bg-gray-200/50 dark:bg-gray-700/50 rounded-lg">
		<div class="flex flex-col gap-1">
			<div class="flex gap-2">
				<Button color="primary" onclick={() => isDrawing = true} disabled={isDrawing}>
					Draw Polygon
				</Button>
				<Button color="secondary" onclick={() => vectorSource.clear()} disabled={isDrawing}>
					Clear
				</Button>
			</div>
			<P size="sm">When drawing:</P>
			<P size="sm">
				<ul class="text-sm list-disc list-inside">
					<li>Click to add points to the current polygon.</li>
					<li>Double click last point to close the polygon</li>
					<li>Polygon can be edited by clicking and dragging points/edges</li>
					<li>Alt+click to remove a point when not drawing</li>
				</ul>
			</P>
		</div>
	</div>
	<View bind:center bind:zoom>
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
