<script lang="ts">
	import { Layer, Map, View, Feature as MapFeature, Overlay } from 'svelte-openlayers';
	import type { Coordinate } from 'ol/coordinate';
	import type { Extent } from 'ol/extent';
	import { LocalStorage } from '$lib/utils/local-storage.svelte';
	import { fromEPSG3857ExtentToSRID4326, fromSRID4326ToEPSG3857 } from '$lib/components/geo-input';
	import type { FeatureLike } from 'ol/Feature.js';
	import { getAllAreas, type GetAllAreasResponseItem } from '$lib/api';

	const mapInformation = new LocalStorage('MAP_LOCATION_AND_ZOOM', {
		zoom: 5,
		center: [0, 0] as Coordinate
	});
	let bbox = $state<Extent | null>(null);
	const bboxSRID4326 = $derived(bbox ? fromEPSG3857ExtentToSRID4326(bbox) : null);
	let areas = $state<GetAllAreasResponseItem[]>([]);

	$effect(() => {
		const extent = bboxSRID4326;
		const zoom = mapInformation.current.zoom;

		if (!extent || zoom < 8) {
			areas = [];
			return;
		}

		const bboxQuery = extent.join(',');
		const controller = new AbortController();
		const timeout = setTimeout(async () => {
			try {
				const { data } = await getAllAreas({
					query: {
						bbox: bboxQuery,
						page_size: 500
					},
					signal: controller.signal
				});

				if (data) {
					areas = data.items;
				}
			} catch (error) {
				if (error instanceof DOMException && error.name === 'AbortError') {
					return;
				}
				throw error;
			}
		}, 300);

		return () => {
			clearTimeout(timeout);
			controller.abort();
		};
	});

	function getFeatureProp(feature: FeatureLike, key: string) {
		return 'get' in feature && typeof feature.get === 'function' ? feature.get(key) : undefined;
	}
</script>

{#snippet areaHoverTooltip(feature: FeatureLike)}
	<p class="text-sm text-center">{getFeatureProp(feature, 'name')}</p>
	<p class="text-xs text-gray-500">{getFeatureProp(feature, 'id')}</p>
{/snippet}

<div class="h-[calc(100vh)] w-full absolute top-0 -z-10">
	<View bind:zoom={mapInformation.current.zoom} bind:center={mapInformation.current.center} bind:bbox>
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
