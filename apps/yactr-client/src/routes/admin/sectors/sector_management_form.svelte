<script lang="ts">
	import type { AreaResponse } from '$lib/api';
	import {
		Button,
		Heading,
		Helper,
		Hr,
		Input,
		Label,
		Select,
		TabItem,
		Tabs
	} from 'flowbite-svelte';
	import { z } from 'zod';
	import { fieldProxy, superForm, type SuperValidated } from 'sveltekit-superforms';
	import type { sectorManagementFormDto } from '$lib/shared/dto/sector_management_form_dto';
	import SectorImagesManager from '$lib/components/ImagesUploading/SectorImagesManager.svelte';
	import { Point as PointInput, Polygon as PolygonInput, LineString as LineStringInput } from '$lib/components/geo-input';

	let {
		data,
		areas = [] as AreaResponse[]
	}: {
		data: SuperValidated<z.infer<typeof sectorManagementFormDto>>;
		areas?: AreaResponse[];
	} = $props();

	const { form, enhance } = superForm(data, {
		dataType: 'json'
	});
	const formDisabled = $derived(!$form.area_id);
	let derived_map_center = $derived(
		areas.find((area) => area.id === $form.area_id)?.location?.coordinates
	);

	let sector_images = fieldProxy(form, 'sector_images');
</script>

<form method="post" enctype="multipart/form-data" use:enhance>
	<div class="grid gap-2">
		<div class="flex flex-col gap-2">
			<Label for="area_select">Area</Label>
			<Select
				bind:value={$form.area_id}
				size="lg"
				name="area_select"
				id="area_select"
				items={areas.map((area) => ({ value: area.id, name: `${area.name} (${area.id})` }))}
				required
			/>
			<Helper class="text-sm">Area the sector should be part of.</Helper>
		</div>
		<div class="flex flex-col gap-2">
			<Label for="name">Name</Label>
			<Input
				type="text"
				name="name"
				id="name"
				bind:value={$form.name}
				required
				disabled={formDisabled}
			/>
		</div>
		<Hr />
		<div>
			<Tabs>
				<TabItem open title="Area" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<PolygonInput
							bind:boundary={$form.sector_area}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Entry point" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<PointInput
							bind:location={$form.entry_point}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Parking" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<PointInput
							bind:location={$form.recommended_parking_location}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Approach" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<LineStringInput
							bind:line={$form.approach_path}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
			</Tabs>
		</div>
		<Hr />
		<div class="flex flex-col gap-2">
			<Heading tag="h2">Images</Heading>
			<SectorImagesManager 
				bind:images={$sector_images} 
				disabled={formDisabled} />
		</div>

		<div class="mt-4 flex justify-end">
			<Button type="submit" color="primary" disabled={formDisabled}>Save</Button>
		</div>
	</div>
</form>
