<script lang="ts">
	import type { Coordinate } from 'ol/coordinate';
	import { Map, Layer, View, Interaction } from 'svelte-openlayers';
	import VectorSource from 'ol/source/Vector';
	import type { MultiPolygon } from '$lib/api';
	import { Button } from '$lib/components/ui/button';
	import { Polygon } from 'ol/geom';
	import { Feature } from 'ol';
	import { untrack } from 'svelte';
	import type { ReactiveCollection } from 'svelte-openlayers/utils';

	let {
		boundary = $bindable(),
		mapCenter = [-74.006, 40.7128],
		disabled = false
	}: {
		boundary?: MultiPolygon | null;
		mapCenter?: Coordinate;
		zoom?: number;
		disabled?: boolean;
	} = $props();

	let vectorSource = $state(new VectorSource());
	let selectedFeatures: ReactiveCollection | null = $state(null);
	
	untrack(() => {
		if (boundary && boundary.coordinates) {
			vectorSource.addFeatures(
				boundary.coordinates.map(polygon => new Feature({
					geometry: new Polygon(polygon)
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
					feature.getGeometry()!.getCoordinates()
				]
			}
		}
	};

	const onModifyEnd = () => {
		boundary = {
				type: "MultiPolygon",
				coordinates: [
					...vectorSource.getFeatures().map(feature => (feature.getGeometry() as Polygon).getCoordinates())
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
					Draw Polygon
				</Button>
				<Button variant="destructive" onclick={() => vectorSource.removeFeatures(selectedFeatures?.getArray()!)} disabled={isDrawing || (selectedFeatures?.getLength() ?? 0) < 1}>
					Delete
				</Button>
			</div>
		</div>
	</div>
	<View center={mapCenter} zoom={12}>
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
