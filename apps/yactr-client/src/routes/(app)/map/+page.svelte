<script lang="ts">
	import { Layer, Map, View, Feature, Interaction, Overlay } from 'svelte-openlayers';
	import VectorSource from 'ol/source/Vector';
	import * as Popover from '$lib/components/ui/popover';
	import * as Card from '$lib/components/ui/card';
	import { Point } from 'ol/geom';
	import { Feature as OlFeature } from 'ol';
	// import { untrack } from 'svelte';
	import { fromSRID4326ToEPSG3857 } from '$lib/components/geo-input';
	import type { Coordinate } from 'ol/coordinate.js';
	import type { FeatureLike } from 'ol/Feature.js';

	let { data } = $props();
	let zoom = $state(5);

  const areas = $derived(data.areas);
  // untrack(() => {
  //   vectorSource.addFeatures(areas.map(area => new OlFeature({
  //     properties: {
	// 			name: area.name
	// 		},
  //     geometry: new Point(fromSRID4326ToEPSG3857(area.location.coordinates!))
  //   })))
  // });;
</script>

<div class="h-[calc(100vh)] w-full absolute top-0 -z-10">
	<View bind:zoom>
		<Map class="h-full w-full">
			<Layer.Tile source="osm" />
      <Layer.Vector>
				{#if zoom >= 5}
					{#each areas as area (area.id)}
						<Feature.Point coordinates={fromSRID4326ToEPSG3857(area.location.coordinates!)} properties={area}>
							<Overlay.Hover positioning="top-center" offset={[50, -10]}>
								<p class="text-sm">{area.name}({area.id})</p>
							</Overlay.Hover>
						</Feature.Point>
					{/each}
				{/if}
			</Layer.Vector>
		</Map>
	</View>
</div>