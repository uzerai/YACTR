<script lang="ts">
	import MultiPolygonSelection from '$lib/components/GeolocationInput/MultiPolygonSelection.svelte';
	import PointSelection from '$lib/components/GeolocationInput/PointSelection.svelte';
import type {
		YactrApiEndpointsSectorsSectorRequestData,
		YactrApiEndpointsAreasAreaResponse,
		YactrApiEndpointsSectorsSectorImageResponseData,
		YactrApiEndpointsSectorsGetSectorByIdData,
		YactrApiEndpointsSectorsGetSectorByIdResponse
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

	let {
		sector = $bindable(),
		loadedImages: loaded_images = $bindable([]),
		areas = [] as YactrApiEndpointsAreasAreaResponse[]
	}: {
		sector?: YactrApiEndpointsSectorsSectorRequestData;
		loadedImages?: YactrApiEndpointsSectorsSectorImageResponseData[];
		areas?: YactrApiEndpointsAreasAreaResponse[];
	} = $props();

	let area_id = $derived(sector?.area_id);

	let boundary = $derived(
		(sector?.sector_area?.coordinates ? [sector.sector_area.coordinates!] : []) as Coordinate[][][]
	);
	let entry_point = $derived(sector?.entry_point?.coordinates as Coordinate);
	let recommended_parking_location = $derived(
		sector?.recommended_parking_location?.coordinates as Coordinate
	);
	let approach_path = $derived((sector?.approach_path?.coordinates ?? []) as Coordinate[]);
	let derived_map_center = $derived(
		areas.find((area) => area.id === area_id)?.location?.coordinates
	);

	let loaded_image_previews: { alt: string; src: string; image_id: string }[] | undefined =
		$derived(
			loaded_images.map((image) => ({
				alt: image.image_id!,
				image_id: image.image_id!,
				src: image.image_url!
			}))
		);
	let primary_image_preview: { alt: string; src: string } | undefined = $derived(
		(sector as YactrApiEndpointsSectorsGetSectorByIdResponse)?.primary_sector_image_url
			? {
					alt: sector!.primary_sector_image_id!,
					src: (sector as YactrApiEndpointsSectorsGetSectorByIdResponse).primary_sector_image_url!
				}
			: undefined
	);
	let otherImagesPreviews: { alt: string; src: string }[] = $derived(loaded_image_previews);

	const setPrimaryImagePreview: ChangeEventHandler<HTMLInputElement> = ({ currentTarget }) => {
		if (currentTarget.files === null) return;

		const file = currentTarget.files[0];

		if (file) {
			const reader = new FileReader();
			reader.addEventListener('load', function () {
				primary_image_preview = {
					alt: file.name,
					src: reader.result as string
				};
			});
			reader.readAsDataURL(file);

			return;
		}
	};

	const setOtherImagesPreviews: ChangeEventHandler<HTMLInputElement> = ({ currentTarget }) => {
		if (currentTarget.files === null) return;
		if (otherImagesPreviews) otherImagesPreviews = [];

		for (let fileIndex = 0; fileIndex < currentTarget.files.length; fileIndex++) {
			const file = currentTarget.files[fileIndex];

			if (file) {
				const reader = new FileReader();
				reader.addEventListener('load', function () {
					console.log('pushing image preview');

					const newImagePreviews = [
						...otherImagesPreviews,
						{
							alt: file.name,
							src: reader.result as string
						}
					];

					otherImagesPreviews = newImagePreviews;
				});
				reader.readAsDataURL(file);
			}
		}

		return;
	};

	let formDisabled = $derived(!area_id);
</script>

<form method="post" enctype="multipart/form-data">
	<div class="grid gap-2">
		<div class="flex flex-col gap-2">
			<Label for="area_select">Area</Label>
			<Input type="hidden" name="area_id" id="area_id" bind:value={area_id} />
			<Select
				bind:value={area_id}
				size="lg"
				name="area_select"
				id="area_select"
				onchange={(e) => (area_id = (e.target as HTMLSelectElement).value)}
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
				value={sector?.name}
				required
				disabled={formDisabled}
			/>
		</div>
		<Hr />
		<div>
			<Input type="hidden" name="sector_area" id="sector_area" value={JSON.stringify(boundary)} />
			<Input
				type="hidden"
				name="entry_point"
				id="entry_point"
				value={JSON.stringify(entry_point)}
			/>
			<Input
				type="hidden"
				name="recommended_parking_location"
				id="recommended_parking_location"
				value={JSON.stringify(recommended_parking_location)}
			/>
			<Input
				type="hidden"
				name="approach_path"
				id="approach_path"
				value={JSON.stringify(approach_path)}
			/>
			<Tabs>
				<TabItem open title="Area" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<MultiPolygonSelection
							bind:boundary
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Entry point" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<PointSelection
							bind:location={entry_point}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Parking" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<PointSelection
							bind:location={recommended_parking_location}
							disabled={formDisabled}
							mapCenter={derived_map_center}
						/>
					</div>
				</TabItem>
				<TabItem title="Approach" disabled={formDisabled}>
					<div class="h-[50dvh] w-full">
						<LineSelection
							bind:line={approach_path}
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
					id="primary_sector_image"
					name="primary_sector_image"
					onchange={setPrimaryImagePreview}
					disabled={formDisabled}
				/>

				<Gallery class="w-full">
					{#if primary_image_preview}
						<img
							alt={primary_image_preview.alt}
							src={primary_image_preview.src}
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
		</div>

		<div class="mt-4 flex justify-end">
			<Button type="submit" color="primary" disabled={formDisabled}>Save</Button>
		</div>
	</div>
</form>
