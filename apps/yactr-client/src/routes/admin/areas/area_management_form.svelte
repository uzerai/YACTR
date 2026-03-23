<script lang="ts">
	import { InfoCircleOutline } from 'flowbite-svelte-icons';
	import { Point as PointInput, MultiPolygon as MultiPolygonInput } from '$lib/components/geo-input';
	import { z } from 'zod';
	import { untrack } from 'svelte';
	import { superForm, type SuperValidated } from 'sveltekit-superforms';
	import { zAreaRequestData } from '$lib/api/generated/zod.gen';
	import * as Form from '$lib/components/ui/form';
	import { Input } from '$lib/components/ui/input';
	import { Textarea } from '$lib/components/ui/textarea';
	import { Button } from '$lib/components/ui/button';

	
	let { data }: { data: SuperValidated<z.infer<typeof zAreaRequestData>> } = $props();

	const form = superForm(untrack(() => data), {
		dataType: 'json'
	});

	const { form: formData, enhance } = form;
</script>

<form method="post" use:enhance>
	<div class="grid gap-6">
		<Form.Field {form} name="name">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>Name</Form.Label>
					<Input {...props} type="string" bind:value={$formData.name} />
				{/snippet}
			</Form.Control>
			<Form.FieldErrors />
		</Form.Field>

		<Form.Field {form} name="description">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>Description</Form.Label>
					<Textarea {...props} class="w-full" rows={4} bind:value={$formData.description} />
				{/snippet}
			</Form.Control>
			<Form.FieldErrors />
		</Form.Field>
	
		<div class="flex flex-col gap-4 md:flex-row">
			<Form.Field {form} name="location">
				<Form.Control>
					<span class="flex items-center gap-1">
						<Form.Label for="location">Location</Form.Label>
						<!-- <InfoCircleOutline class="size-4 shrink-0" /> -->
						<!-- <Tooltip class="max-w-80">
							Position to be used (and searchable by) on world-map view.
						</Tooltip> -->
					</span>
					<div class="h-[50dvh] w-full">
						<PointInput bind:location={$formData.location} mapCenter={$formData.location?.coordinates} />
					</div>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>
			<div class="flex w-full flex-col gap-2 md:w-1/2">
				<span class="flex items-center gap-1">
					<!-- <Label for="boundary">Boundary</Label> -->
					<InfoCircleOutline class="size-4 shrink-0" />
					<!-- <Tooltip class="max-w-80">General area outline; used to enforce borders of sectors</Tooltip> -->
					</span>
				<div class="h-[calc(50dvh)]">
					<MultiPolygonInput bind:boundary={$formData.boundary} mapCenter={$formData.location?.coordinates} />
				</div>
			</div>
		</div>
	</div>
	<div class="mt-4 flex justify-end">
		<Button type="submit" color="primary">Save</Button>
	</div>
</form>
