<script lang="ts">
	import VectorSource from 'ol/source/Vector';
	import type { Coordinate } from 'ol/coordinate';
	import { LineString } from 'ol/geom';
	import { Feature, type View as OLView } from 'ol';
	import { untrack } from 'svelte';
	import { Interaction, Layer, View, Map } from 'svelte-openlayers';
	import type { LineString as LineStringGeoJSON } from '$lib/api';
	import { fromEPSG3857ToSRID4326, fromSRID4326ToEPSG3857 } from '$lib/components/geo-input';

	let {
		line = $bindable(),
		mapCenter = [-74.006, 40.7128, 0],
		zoom = 12,
		disabled = false
	}: {
		line?: LineStringGeoJSON | null;
		mapCenter?: Coordinate;
		zoom?: number;
		disabled?: boolean;
	} = $props();

	let vectorSource = $state(new VectorSource());
	let view = $state<OLView | null>(null);

	$effect(() => {
		if (mapCenter) {
			view?.setCenter(fromSRID4326ToEPSG3857(mapCenter));
		}
	});

	untrack(() => {
		if (line && line.coordinates) {
			vectorSource.addFeature(
				new Feature({
					geometry: new LineString(line.coordinates.map(fromSRID4326ToEPSG3857))
				}));
		}
	});

	const onDrawEnd = ({ feature }: { feature: Feature<LineString> }) => {
		vectorSource.clear();

		if (feature.getGeometry() !== undefined) {
			line = {
				type: "LineString",
				coordinates: [
					...feature.getGeometry()!.getCoordinates()
						.map(fromEPSG3857ToSRID4326)
				]
			}
		}
	};
	
	const onModifyEnd = () => {
		if (vectorSource.getFeatures().length < 1) return;

		line = {
				type: "LineString",
				coordinates: (vectorSource.getFeatures()
					.at(0)?.getGeometry() as LineString)?.getCoordinates()
					.map(fromEPSG3857ToSRID4326) ?? []
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
		</div>
	{/if}
	
	<View bind:view {zoom}>
		<Map class="h-full w-full">
			<Layer.Tile source="osm" />

			<Layer.Vector bind:source={vectorSource}>
				<Interaction.Draw minPoints={2} type="LineString" bind:source={vectorSource} onDrawEnd={onDrawEnd} />
				<Interaction.Modify bind:source={vectorSource} onModifyEnd={onModifyEnd} />
			</Layer.Vector>
		</Map>
	</View>
</div>
