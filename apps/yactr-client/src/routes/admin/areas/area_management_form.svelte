<script lang="ts">
	import { Input, Label, Button, Textarea, Tooltip, Hr } from 'flowbite-svelte';
	import { InfoCircleOutline } from 'flowbite-svelte-icons';
	import MultiPolygonSelection from '$lib/components/GeolocationInput/MultiPolygonSelection.svelte';
	import PointSelection from '$lib/components/GeolocationInput/PointSelection.svelte';
	import type { Infer } from 'zod';
	import { superForm, type SuperValidated } from 'sveltekit-superforms';
	import { zYactrApiEndpointsAreasAreaRequestData } from '$lib/api/generated/zod.gen';
	import SuperDebug from 'sveltekit-superforms/SuperDebug.svelte';
	let { data } : { data : SuperValidated<Infer<typeof zYactrApiEndpointsAreasAreaRequestData>> } = $props();

	const { form, errors, enhance } = superForm(data, {
		dataType: 'json'
	});

</script>

<SuperDebug bind:data={$form} />

<form method="post" use:enhance>
	<div class="grid gap-6">
		<div class="flex w-1/2 flex-col gap-2">
			<Label for="name">Name</Label>
			<Input type="text" name="name" id="name" bind:value={$form.name} required />
		</div>
		<div class="flex w-1/2 flex-col gap-2">
			<Label for="description">Description</Label>
			<Textarea
				name="description"
				id="description"
				class="w-full"
				rows={4}
				bind:value={$form.description as string | undefined} // Note to self; Textarea does _SUPPORT_ nullish values, just doesn't expose it as an accepted type
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
			<div class="h-[50dvh]">
				<PointSelection bind:location={$form.location} mapCenter={$form.location?.coordinates} />
			</div>
		</div>
		<div class="flex w-full flex-col gap-2 md:w-1/2">
			<span class="flex items-center gap-1"
				><Label for="boundary">Boundary</Label>
				<InfoCircleOutline class="size-4 shrink-0" />
				<Tooltip class="max-w-80">General area outline; used to enforce borders of sectors</Tooltip
				></span
			>
			<div class="h-[calc(50dvh)]">
				<MultiPolygonSelection bind:boundary={$form.boundary} mapCenter={$form.location?.coordinates} />
			</div>
		</div>
	</div>
	<div class="mt-4 flex justify-end">
		<Button type="submit" color="primary">Save</Button>
	</div>
</form>
