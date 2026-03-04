<script lang="ts">
	import { Map, Layer, Feature, Overlay, Interaction } from 'svelte-openlayers';
	import { createCircleStyle, createStyle, ReactiveCollection } from 'svelte-openlayers/utils';
	import type { Coordinate } from 'ol/coordinate';
	import type { Feature as OlFeature } from 'ol';
	import { Button } from 'flowbite-svelte';

	let {
		boundary = $bindable(),
		mapCenter: center = $bindable([-74.006, 40.7128]),
		zoom = $bindable(12),
		disabled = false
	}: {
		boundary?: Coordinate[][][];
		mapCenter?: Coordinate;
		zoom?: number;
		disabled?: boolean;
	} = $props();

	let polygons = $derived(
		boundary?.map((polygon) => ({
			properties: {
				id: crypto.randomUUID(),
				type: 'polygon',
				coordinates: polygon
			},
			feature: null as OlFeature
		})) ?? []
	);
	let newPolygon = $state<Coordinate[]>();

	let points = $derived(
		polygons.flatMap((polygon) =>
			polygon.properties.coordinates.flat(1).map((point) => ({
				properties: {
					id: crypto.randomUUID(),
					polygonId: polygon.properties.id,
					type: 'point',
					coordinates: point
				},
				feature: null as OlFeature
			}))
		) ?? []
	);

	let mode: 'create' | 'edit' | undefined = $state();

	let selectedFeatures: ReactiveCollection<OlFeature> = $state(null);

	const saveNewPolygon = () => {
		//TODO: explore if I can do this without updating polygons _first_; and instead only update the boundary causing the refresh of derived data.
		polygons = [
			...polygons,
			{
				properties: { id: crypto.randomUUID(), type: 'polygon', coordinates: [newPolygon!] },
				feature: null
			}
		];
		mode = undefined;
		newPolygon = undefined;
		selectedFeatures?.clear();
		boundary = polygons.map((polygon) => polygon.properties.coordinates);
	};

	const setToCreateMode = () => {
		mode = 'create';
		newPolygon = [];
	};

	const mapClick = ({ coordinate }: { coordinate: Coordinate }) => {
		if (mode === 'create' && !disabled) {
			newPolygon = [...(newPolygon ?? []), coordinate];
		}
	};

	// TODO: Refactor this to _force_ type safety only to work with polygon features.
	const deletePolygon = (feature: OlFeature) => {
		// This should never happen (see filter on select interaction) but javascript devs are a breed apart.
		if (feature.getProperties().type !== 'polygon') return;

		polygons = polygons.filter((polygon) => polygon.properties.id !== feature.getProperties().id);

		selectedFeatures?.clear();
	};

	const polygonStyle = createStyle({
		fill: {
			color: 'rgba(99, 102, 241, 0.3)'
		},
		stroke: {
			color: '#4338ca',
			width: 2
		}
	});

	const newPolygonStyle = createStyle({
		fill: {
			color: '#ff0000'
		},
		stroke: {
			color: '#ff0000',
			width: 2
		}
	});

	const pointStyle = createCircleStyle({
		radius: 4,
		fill: '#4338ca', // Uses --ol-color-primary
		stroke: '#ffffff',
		strokeWidth: 2
	});
</script>

<div class={`relative h-full w-full overflow-hidden rounded-lg border`}>
	{#if disabled}
		<div
			class="absolute top-0 right-0 z-20 h-full w-full cursor-not-allowed bg-gray-300 opacity-30"
		></div>
	{:else}
		<div class="absolute top-0 right-0 z-20 m-4 flex gap-1">
			{#if mode === 'create'}
				<Button onclick={saveNewPolygon} color="blue">Save</Button>
			{:else}
				<Button onclick={setToCreateMode} color="green">Add Polygon</Button>
			{/if}
			<Button
				onclick={() => deletePolygon(selectedFeatures?.item(0)!)}
				disabled={selectedFeatures?.getLength() === 0 || mode === 'create'}
				color="red"
			>
				Delete
			</Button>
		</div>
	{/if}

	<Map.Root class="h-full w-full" onClick={mapClick}>
		<Map.View bind:center bind:zoom />
		<Layer.Tile source="osm" />

		<Layer.Vector style={polygonStyle}>
			{#each polygons as polygon}
				<Feature.Polygon
					coordinates={polygon.properties.coordinates}
					properties={polygon.properties}
					feature={polygon.feature}
				/>
			{/each}
		</Layer.Vector>

		{#if newPolygon}
			<Layer.Vector style={newPolygonStyle}>
				{#each newPolygon as coordinate}
					<Feature.Point coordinates={coordinate} properties={{ type: 'point' }} feature={null} />
				{/each}
				<Feature.Polygon
					coordinates={[newPolygon]}
					properties={{ type: 'polygon' }}
					feature={null}
				/>
			</Layer.Vector>
		{/if}

		<Layer.Vector style={pointStyle}>
			{#each points as point}
				<Feature.Point
					coordinates={point.properties.coordinates}
					properties={point.properties}
					feature={point.feature}
				/>
			{/each}
		</Layer.Vector>
		<Interaction.Select
			filter={(feature: OlFeature) => feature.getProperties().type === 'polygon'}
			bind:selectedFeatures
			multi={false}
		/>
		<Overlay.TooltipManager hoverTooltip={true} selectTooltip={false} bind:selectedFeatures>
			{#snippet hoverSnippet(feature)}
				{@const props = feature.getProperties()}
				<p>{props.name}</p>
			{/snippet}
		</Overlay.TooltipManager>
	</Map.Root>
</div>
