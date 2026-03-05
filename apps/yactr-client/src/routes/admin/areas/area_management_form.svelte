<script lang="ts">
	import type { YactrApiEndpointsAreasAreaRequestData } from '$lib/api';
	import { Input, Label, Button, Textarea, Tooltip, Heading, Hr } from 'flowbite-svelte';
	import { InfoCircleOutline } from 'flowbite-svelte-icons';
	import MultiPolygonSelection from '$lib/components/GeolocationInput/MultiPolygonSelection.svelte';
	import PointSelection from '$lib/components/GeolocationInput/PointSelection.svelte';
	import type { Coordinate } from 'ol/coordinate';

	let { area = $bindable() }: { area?: YactrApiEndpointsAreasAreaRequestData } = $props();

	let boundary = $derived(area?.boundary?.coordinates as Coordinate[][][]);
	let location = $derived(area?.location?.coordinates as Coordinate);
</script>

<form method="post">
	<div class="grid gap-6">
		<div class="flex w-1/2 flex-col gap-2">
			<Label for="name">Name</Label>
			<Input type="text" name="name" id="name" value={area?.name} required />
		</div>
		<div class="flex w-1/2 flex-col gap-2">
			<Label for="description">Description</Label>
			<Textarea
				name="description"
				id="description"
				class="w-full"
				rows={4}
				value={area?.description ?? undefined}
			/>
		</div>
	</div>
	<Hr />
	<div class="flex flex-col gap-4 md:flex-row">
		<div class="flex w-full flex-col gap-2 md:w-1/2">
			<span class="flex items-center gap-1"
				><Label for="location">Location</Label>
				<InfoCircleOutline class="size-4 shrink-0" />
				<Tooltip class="max-w-80"
					>Position to be used (and searchable by) on world-map view.</Tooltip
				></span
			>
			<Input type="hidden" name="location" id="location" value={JSON.stringify(location)} />
			<div class="h-[50dvh]">
				<PointSelection bind:location mapCenter={area?.location?.coordinates} />
			</div>
		</div>
		<div class="flex w-full flex-col gap-2 md:w-1/2">
			<span class="flex items-center gap-1"
				><Label for="boundary">Boundary</Label>
				<InfoCircleOutline class="size-4 shrink-0" />
				<Tooltip class="max-w-80">General area outline; used to enforce borders of sectors</Tooltip
				></span
			>
			<Input type="hidden" name="boundary" id="boundary" value={JSON.stringify(boundary)} />
			<div class="h-[calc(50dvh)]">
				<MultiPolygonSelection bind:boundary mapCenter={area?.location?.coordinates} />
			</div>
		</div>
	</div>
	<div class="mt-4 flex justify-end">
		<Button type="submit" color="primary">Save</Button>
	</div>
</form>
