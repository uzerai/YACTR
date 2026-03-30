<script lang="ts">
	import { Layer, Map, View } from 'svelte-openlayers';
	import VectorSource from 'ol/source/Vector';
	import { Point } from 'ol/geom';
	import { Feature as OlFeature } from 'ol';
	import { untrack } from 'svelte';
	import { fromSRID4326ToEPSG3857 } from '$lib/components/geo-input';

	let { data } = $props();

  const areas = $derived(data.areas);

  let vectorSource = $state(new VectorSource());
  
  $inspect(areas);

  untrack(() => {
    vectorSource.addFeatures(areas.map(area => new OlFeature({
      name: area.name,
      description: area.description,
      geometry: new Point(fromSRID4326ToEPSG3857(area.location.coordinates!))
    })))
  });;
</script>

<div class="h-[calc(100vh)] w-full absolute top-0 -z-10">
	<View zoom={12}>
		<Map class="h-full w-full">
			<Layer.Tile source="osm" />
      <Layer.Vector bind:source={vectorSource} />
		</Map>
	</View>
</div>