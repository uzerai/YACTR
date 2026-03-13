<script lang="ts">
	import {
		getSectorById,
		type RouteResponse,
		type SectorImageResponseData,
		type SectorResponse
	} from '$lib/api';
	import { ClimbingType, ClimbingType as ClimbingTypeObj } from '$lib/api/generated/types.gen';
	import {
		Button,
		Card,
		Fileupload,
		Helper,
		Hr,
		Input,
		Label,
		P,
		Select,
		TabItem,
		Tabs,
		Textarea,
		Toggle,
		Tooltip
	} from 'flowbite-svelte';
	import RouteEditor from '$lib/components/RouteTopoEditor/route_topo_editor.svelte';
	import { InfoCircleOutline } from 'flowbite-svelte-icons';
	import { z } from 'zod';
	import SuperDebug, { superForm, type SuperValidated } from 'sveltekit-superforms';
	import type { routeManagementFormDto } from '$lib/shared/dto/route_management_form_dto';

	let {
		data,
		sectors = []
	}: {
		data: SuperValidated<z.infer<typeof routeManagementFormDto>>;
		sectors: SectorResponse[];
	} = $props();

	const { form, enhance } = superForm(data, {
		dataType: 'json'
	})

	const climbing_types = Object.values(ClimbingTypeObj);
	let form_disabled = $derived(!$form.sector_id);
	let is_multipitch = $derived(($form.pitches?.length ?? 0) > 1);

	let selected_sector = $state<SectorResponse>();

	let sector_images = $derived<
		Array<SectorImageResponseData & Partial<{ is_primary: boolean }>>
	>(
		// Here we just splice the primary image indications from response.
		selected_sector?.sector_images
			?.map((image: SectorImageResponseData) => ({ ...image, is_primary: image.image_id === selected_sector?.primary_sector_image_id })) ?? []
	);
	
	let selected_sector_topo_image = $derived<string | undefined>(
		sector_images?.find((image) => image.image_id === $form.sector_topo_image_id)?.image_url);


	// let route_image_input_html = $state<HTMLInputElement>();
	// let route_image_uploader = $state<HTMLInputElement>();
	// let route_image_overlay_input_html = $state<HTMLInputElement>();
	// let sector_image_overlay_input_html = $state<HTMLInputElement>();

	// const add_new_pitch = () => {
	// 	if (route.pitches) {
	// 		route.pitches.push({
	// 			pitch_order: route.pitches.length
	// 		});
	// 	} else {
	// 		route.pitches = [{}];
	// 	}
	// };

	$effect(() => {
		if ($form.sector_id) {
			getSectorById({
				path: {
					sector_id: $form.sector_id
				}
			}).then((result: Awaited<ReturnType<typeof getSectorById>>) => {
				const { data, response } = result;
				if (response.ok) {
					selected_sector = data;
				}
			});
		}
	});
</script>

<SuperDebug data={$form} />

<form method="post" class="" enctype="multipart/form-data">
	<div class="grid gap-6 md:grid-cols-2">
		<div class="flex flex-col gap-4">
			<div class="flex flex-col gap-2">
				<Label for="sector_id">Sector</Label>
				<Select
					bind:value={$form.sector_id}
					size="lg"
					name="sector_id"
					id="sector_id"
					required
					items={sectors.map((sector) => ({
						value: sector.id,
						name: `${sector.name} (${sector.id})`
					}))}
				/>
				<Helper>Sectors can be filtered with the keyboard</Helper>
			</div>
			<Hr />
			<div class="flex items-baseline-last gap-5">
				<div class="flex w-1/2 flex-col gap-2">
					<Label for="climbing_type">Climbing type</Label>
					<Select
						bind:value={$form.type}
						name="climbing_type"
						id="climbing_type"
						disabled={form_disabled || is_multipitch}
						required
						items={climbing_types.map((climbing_type) => ({
							value: climbing_type,
							name: climbing_type
						}))}
					/>
				</div>
				<div class="flex items-center gap-2">
					<Label for="is_multipitch">
						<span class="flex gap-1">Multipitch <InfoCircleOutline /></span>
						<Tooltip class="text-xs" placement="top"
							>If enabled, the type will be set by the types in the pitches below.</Tooltip
						>
					</Label>
					<Toggle
						name="is_multipitch"
						bind:checked={is_multipitch}
						disabled={$form.type === ClimbingType.BOULDER}
					/>
				</div>
			</div>
			<div class="flex flex-col gap-2">
				<Label for="name">Name</Label>
				<Input name="name" bind:value={$form.name} required disabled={form_disabled} />
			</div>
			<div class="flex flex-col gap-2">
				<Label for="height">Height</Label>
				<Input name="height" type="number" min="0" disabled={form_disabled || is_multipitch} />
			</div>
			<div class="flex flex-col gap-2">
				<Label for="description">Description</Label>
				<Textarea
					name="description"
					class="w-1/2"
					bind:value={$form.description as string | undefined}
					disabled={form_disabled}
				/>
			</div>
			<div class="flex gap-2">
				<div class="flex flex-col gap-2">
					<Label for="grade">Grade</Label>
					<Input name="grade" bind:value={$form.grade as number | undefined} disabled={form_disabled} />
				</div>
				<div class="flex flex-col gap-2">
					<Label for="number_of_bolts">Number of bolts</Label>
					<Input name="number_of_bolts" type="number" disabled={form_disabled} />
				</div>
			</div>
			<div class="flex flex-col gap-2">
				<Label for="bolter_name">Bolted by</Label>
				<Input name="bolter_name" bind:value={$form.bolter_name as string | undefined} disabled={form_disabled} />
			</div>
			<div class="flex flex-col gap-2">
				<Label for="first_ascent_climber_name">First ascended by</Label>
				<Input
					name="first_ascent_climber_name"
					bind:value={$form.first_ascent_climber_name as string | undefined}
					disabled={form_disabled}
				/>
			</div>

			{#if is_multipitch}
				<Hr>Pitches</Hr>
				{#each $form.pitches as $pitch, index}
					<Card class="p-2">
						<div class="flex gap-2">
							<Label for="pitch_order">Order</Label>
							<Input type="number" name="pitch_order" value={index} required />
						</div>
						<div class="flex gap-2">
							<Label for="pitch_type">Type</Label>
							<Select
								bind:value={$pitch.type}
								name="pitch_type"
								id="pitch_type"
								required
								items={climbing_types.map((climbing_type) => ({
									value: climbing_type,
									name: climbing_type
								}))}
							/>
						</div>
						<div class="flex flex-col gap-2">
							<Label for="pitch_name">Name</Label>
							<Input name="pitch_name" bind:value={$pitch.name as string | undefined} required />
						</div>
						<div class="flex gap-2">
							<Label for="pitch_height">Height</Label>
							<Input type="number" name="pitch_height" bind:value={$pitch.height as number | undefined} required />
						</div>
						<div class="flex gap-2">
							<Label for="pitch_grade">Grade</Label>
							<Input name="pitch_grade" required />
						</div>
						<div class="flex flex-col gap-2">
							<Label for="pitch_description">Description</Label>
							<Input name="pitch_description" bind:value={$pitch.description as string | undefined} />
						</div>
					</Card>
				{/each}
				<!-- <Button type="button" color="secondary" onclick={add_new_pitch}>Add pitch</Button> -->
			{/if}
		</div>
		<div class="flex flex-col gap-2">
			<Tabs>
				<TabItem open title="Sector topo" disabled={form_disabled}>
					<div class="flex flex-col gap-2">
						<Label for="sector_image_selector">Sector image selection</Label>
						<Select
							name="sector_image_selector"
							items={sector_images?.map(({ image_id, image_url, is_primary }) => ({
								value: image_id,
								name: `${is_primary ? 'Primary image:' : 'Secondary image:'} ${image_id}`
							}))}
							bind:value={$form.sector_topo_image_id}
						/>
						<RouteEditor
							bind:image={selected_sector_topo_image}
							bind:points={$form.sector_topo_line_points}
							bind:svg_file={$form.sector_topo_image_overlay_url}
							// bind:svg_output_file_upload={sector_image_overlay_input_html}
						/>
					</div>
				</TabItem>
				<!-- <TabItem title="Route unique topo" disabled={form_disabled}>
					<div class="flex flex-col gap-2">
						<Label for="route_image_uploader">Route unique topo image upload</Label>
						<P size="xs">Intended for if the sector image doesn't depict the route in full</P>
						<Fileupload
							id="route_image_uploader"
							type="file"
							name="route_image_uploader"
							accept=".jpeg,.jpg,.png,.webp"
							onchange={(event) => {
								// If this upload changes; duplicate the files to route_image_input_html input.
								if (event.currentTarget && event.currentTarget.files && route_image_input_html) {
									const dataTransfer = new DataTransfer();

									if (event.currentTarget.files[0]) {
										dataTransfer.items.add(event.currentTarget.files[0]);
										console.info(
											'Setting html files for route_image_input_html',
											dataTransfer.files
										);
										route_image_input_html.files = dataTransfer.files;

										const reader = new FileReader();
										reader.onload = (event) => {
											route.topo_image_url = event.target?.result as string;
										};
										reader.readAsDataURL(event.currentTarget.files[0]);
									}
								}
							}}
							disabled={form_disabled}
						/>
						<RouteEditor
							bind:image={$form.topo_image_url}
							bind:points={route.topo_line_points}
							bind:svg_file={$form.topo_image}
							bind:svg_output_file_upload={route_image_overlay_input_html}
						/>
					</div>
				</TabItem> -->
			</Tabs>
		</div>
	</div>
	<div class="mt-4 flex justify-end">
		<Button type="submit" color="primary">Save</Button>
	</div>
</form>
