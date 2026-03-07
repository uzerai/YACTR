<script lang="ts">
	import MultiPolygonSelection from '$lib/components/GeolocationInput/MultiPolygonSelection.svelte';
	import PointSelection from '$lib/components/GeolocationInput/PointSelection.svelte';
import type {
		SectorRequestData,
		AreaResponse,
		SectorImageResponseData,
		GetSectorByIdData,
		GetSectorByIdResponse,
		MultiPolygon,
		Point,
		LineString
	} from '$lib/api';
	import type { Coordinate } from 'ol/coordinate';
	import {
		Button,
		Fileupload,
		Gallery,
		Helper,
		Hr,
		Input,
		Label,
		Select,
		TabItem,
		Tabs
	} from 'flowbite-svelte';
	import LineSelection from '$lib/components/GeolocationInput/LineSelection.svelte';
	import type { ChangeEventHandler } from 'svelte/elements';
	import type { zSectorRequestData } from '$lib/api/generated/zod.gen';
	import { z } from 'zod';
	import type { SuperValidated } from 'sveltekit-superforms';
	import SuperDebug, { superForm } from 'sveltekit-superforms';
	import PolygonSelection from '$lib/components/GeolocationInput/PolygonSelection.svelte';

	let {
		data,
		areas = [] as AreaResponse[]
	}: {
		data: SuperValidated<z.infer<typeof zSectorRequestData>>;
		areas?: AreaResponse[];
	} = $props();

	const { form, enhance } = superForm(data, {
		dataType: 'json'
	});

	let derived_map_center = $derived(
		areas.find((area) => area.id === $form.area_id)?.location?.coordinates
	);

	// let boundary = $derived(
	// 	(sector?.sector_area?.coordinates ? [sector.sector_area.coordinates!] : []) as Coordinate[][][]
	// );
	// let entry_point = $derived(sector?.entry_point?.coordinates as Coordinate);
	// let recommended_parking_location = $derived(
	// 	sector?.recommended_parking_location?.coordinates as Coordinate
	// );
	// let approach_path = $derived((sector?.approach_path?.coordinates ?? []) as Coordinate[]);
	

	// let loaded_image_previews: { alt: string; src: string; image_id: string }[] | undefined =
	// 	$derived(
	// 		loaded_images.map((image) => ({
	// 			alt: image.image_id!,
	// 			image_id: image.image_id!,
	// 			src: image.image_url!
	// 		}))
	// 	);
	// let primary_image_preview: { alt: string; src: string } | undefined = $derived(
	// 	(sector as GetSectorByIdResponse)?.primary_sector_image_url
	// 		? {
	// 				alt: sector!.primary_sector_image_id!,
	// 				src: (sector as GetSectorByIdResponse).primary_sector_image_url!
	// 			}
	// 		: undefined
	// );
	// let otherImagesPreviews: { alt: string; src: string }[] = $derived(loaded_image_previews);

	// const setPrimaryImagePreview: ChangeEventHandler<HTMLInputElement> = ({ currentTarget }) => {
	// 	if (currentTarget.files === null) return;

	// 	const file = currentTarget.files[0];

	// 	if (file) {
	// 		const reader = new FileReader();
	// 		reader.addEventListener('load', function () {
	// 			primary_image_preview = {
	// 				alt: file.name,
	// 				src: reader.result as string
	// 			};
	// 		});
	// 		reader.readAsDataURL(file);

	// 		return;
	// 	}
	// };

	// const setOtherImagesPreviews: ChangeEventHandler<HTMLInputElement> = ({ currentTarget }) => {
	// 	if (currentTarget.files === null) return;
	// 	if (otherImagesPreviews) otherImagesPreviews = [];

	// 	for (let fileIndex = 0; fileIndex < currentTarget.files.length; fileIndex++) {
	// 		const file = currentTarget.files[fileIndex];

	// 		if (file) {
	// 			const reader = new FileReader();
	// 			reader.addEventListener('load', function () {
	// 				console.log('pushing image preview');

	// 				const newImagePreviews = [
	// 					...otherImagesPreviews,
	// 					{
	// 						alt: file.name,
	// 						src: reader.result as string
	// 					}
	// 				];

	// 				otherImagesPreviews = newImagePreviews;
	// 			});
	// 			reader.readAsDataURL(file);
	// 		}
	// 	}

	// 	return;
	// };

	let formDisabled = $derived(!$form.area_id);
</script>

<SuperDebug data={form} />

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
						<PolygonSelection
							bind:boundary={$form.sector_area}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Entry point" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<PointSelection
							bind:location={$form.entry_point}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Parking" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<PointSelection
							bind:location={$form.recommended_parking_location}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Approach" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<LineSelection
							bind:line={$form.approach_path}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
			</Tabs>
		</div>
		<Hr />
		<!-- <div class="flex gap-4">
			<div class="flex w-1/2 flex-col gap-2">
				<Label for="primary_sector_image">Primary sector image</Label>
				<Fileupload
					type="file"
					onchange={setPrimaryImagePreview}
					disabled={formDisabled}
				/>

				<Gallery class="w-full">
					{#if $form.primary_sector_image_id}
						<img
							alt={$form.primary_sector_image_id}
							src={$form.primary_sector_image_url}
							class="rounded-xl"
						/>
					{/if}
				</Gallery>
			</div>
			<div class="flex flex-1 flex-col gap-2">
				<Label for="sector_images">Other images</Label>
				<Fileupload
					type="file"
					id="sector_images"
					name="sector_images"
					multiple
					disabled={formDisabled}
					onchange={setOtherImagesPreviews}
				/>
				<Gallery items={otherImagesPreviews} class="grid-cols-3 gap-2" />
			</div>
		</div> -->

		<div class="mt-4 flex justify-end">
			<Button type="submit" color="primary" disabled={formDisabled}>Save</Button>
		</div>
	</div>
</form>
