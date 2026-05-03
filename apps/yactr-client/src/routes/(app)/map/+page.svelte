<script lang="ts">
	import { Layer, Map, View, Feature as MapFeature, Overlay } from 'svelte-openlayers';
	import type { FeatureLike } from 'ol/Feature';
	import type { Coordinate } from 'ol/coordinate';
	import { LocalStorage } from '$lib/utils/local-storage.svelte';
	// import { untrack } from 'svelte';
	import { fromEPSG3857ToSRID4326, fromSRID4326ToEPSG3857 } from '$lib/components/geo-input';

	let { data } = $props();
	const mapInformation = new LocalStorage('MAP_LOCATION_AND_ZOOM', {
		zoom: 5,
		center: [0, 0] as Coordinate
	})
	const areas = $derived(data.areas);

	function getFeatureProp(feature: FeatureLike, key: string) {
		return 'get' in feature && typeof feature.get === 'function' ? feature.get(key) : undefined;
	}
</script>

{#snippet areaHoverTooltip(feature: FeatureLike)}
	<p class="text-sm text-center">{getFeatureProp(feature, 'name')}</p>
	<p class="text-xs text-gray-500">{getFeatureProp(feature, 'id')}</p>
{/snippet}

<div class="h-[calc(100vh)] w-full absolute top-0 -z-10">
	<View bind:zoom={mapInformation.current.zoom} bind:center={mapInformation.current.center}>
		<Map class="h-full w-full" controls={{ zoom: false }}>
			<Layer.Tile source="osm" />
      <Layer.Vector>
				{#if mapInformation.current.zoom >= 8}
					{#each areas as area (area.id)}
						<MapFeature.Point coordinates={fromSRID4326ToEPSG3857(area.location.coordinates!)} properties={area} />
					{/each}
				{/if}
			</Layer.Vector>

			<Overlay.TooltipManager
				hoverSnippet={areaHoverTooltip}
				hoverPositioning="bottom-center"
				selectTooltip={false}
			/>
		</Map>
	</View>
</div>