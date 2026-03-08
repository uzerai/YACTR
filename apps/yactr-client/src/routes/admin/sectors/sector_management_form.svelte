<script lang="ts">
	import PointSelection from '$lib/components/GeolocationInput/PointSelection.svelte';
	import type { AreaResponse } from '$lib/api';
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
	import { z } from 'zod';
	import SuperDebug, { fileProxy, filesProxy, superForm, type SuperValidated } from 'sveltekit-superforms';
	import PolygonSelection from '$lib/components/GeolocationInput/PolygonSelection.svelte';
	import type { sectorRequestWithImages } from '$lib/server/sector_request_with_images';

	let {
		data,
		areas = [] as AreaResponse[]
	}: {
		data: SuperValidated<z.infer<typeof sectorRequestWithImages>>;
		areas?: AreaResponse[];
	} = $props();

	const { form, enhance } = superForm(data, {
		dataType: 'json'
	});

	let derived_map_center = $derived(
		areas.find((area) => area.id === $form.area_id)?.location?.coordinates
	);

	const primary_image_file_proxy = fileProxy(form, 'primary_sector_image');
	const primary_image_preview = $derived.by(() => {
		if (!$form.primary_sector_image_url && $primary_image_file_proxy.length === 0) return undefined;

		if ($primary_image_file_proxy?.[0]?.name) {
			return {
				alt: $primary_image_file_proxy[0]!.name,
				src: URL.createObjectURL($primary_image_file_proxy[0]!)
			};
		}

		return {
			alt: $form.primary_sector_image_id!,
			src: $form.primary_sector_image_url
		};
	});

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
		<div class="flex gap-4">
			<div class="flex w-1/2 flex-col gap-2">
				<Label for="primary_sector_image">Primary sector image</Label>
				<Fileupload
					type="file"
					accept="image/*"
					name="primary_sector_image"
					disabled={formDisabled}
					bind:files={$primary_image_file_proxy}
				/>

				<Gallery class="w-full">
					{#if primary_image_preview}
						<img
							alt={primary_image_preview?.alt}
							src={primary_image_preview?.src}
							class="rounded-xl"
						/>
					{/if}
				</Gallery>
			</div>
			<!-- <div class="flex flex-1 flex-col gap-2">
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
			</div> -->
		</div>

		<div class="mt-4 flex justify-end">
			<Button type="submit" color="primary" disabled={formDisabled}>Save</Button>
		</div>
	</div>
</form>
