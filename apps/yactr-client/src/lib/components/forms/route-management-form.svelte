<script lang="ts">
	import {
		getSectorById,
		type SectorImageResponseData,
		type SectorResponse
	} from '$lib/api';
	import { ClimbingType } from '$lib/api/generated/types.gen';
	import { m } from '$lib/paraglide/messages.js';
	import { TopoEditor } from '$lib/components/topo-editor';
	import { untrack } from 'svelte';
	import { z } from 'zod';
	import { superForm, type SuperValidated } from 'sveltekit-superforms';
	import { routeManagementFormDto } from '$lib/components/forms';
	import * as Form from '$lib/components/ui/form';
	import * as Select from '$lib/components/ui/native-select';
	import * as Tabs from '$lib/components/ui/tabs';
	import { Separator } from '$lib/components/ui/separator';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Textarea } from '$lib/components/ui/textarea';
	import { SuperDebugHelper, useSuperDebugForm } from '$lib/components/forms/util/super-debug-helper';

	let {
		data,
		sectors = []
	}: {
		data: SuperValidated<z.infer<typeof routeManagementFormDto>>;
		sectors: SectorResponse[];
	} = $props();

	const form = superForm(untrack(() => data), {
		dataType: 'json'
	});

	const { form: formData, enhance, errors, allErrors, message } = form;

	useSuperDebugForm(formData);
	
	const climbingTypes = Object.values(ClimbingType);
	let formDisabled = $derived(!$formData.sector_id);

	let selectedSector = $state<SectorResponse>();

	let sectorImages = $derived<
		Array<SectorImageResponseData & Partial<{ is_primary: boolean }>>
	>(
		selectedSector?.sector_images
			?.map((image: SectorImageResponseData) => ({ ...image, is_primary: image.image_id === selectedSector?.primary_sector_image_id })) ?? []
	);
	
	let selectedSectorTopoImage = $derived<string | undefined>(
		sectorImages?.find((image) => image.image_id === $formData.sector_topo_image_id)?.image_url);
	let routeTopoSource = $derived<File | string | undefined>(
		$formData.topo_image ?? $formData.topo_image_url ?? undefined
	);

	const updateSelectedSector = async (sectorId: string) => {
		const result = await getSectorById({
			path: {
				sector_id: sectorId
			}
		});

		if (result.response.ok && result.data) {
			selectedSector = result.data;
			$formData.sector_topo_image_id = result.data.primary_sector_image_id;
		}
	};

	const handleSectorChange = (event: Event) => {
		const select = event.currentTarget as HTMLSelectElement | null;
		if (!select?.value) return;
		void updateSelectedSector(select.value);
	};

	const handleRouteTopoFileChange = (event: Event) => {
		const input = event.currentTarget as HTMLInputElement | null;
		const file = input?.files?.[0];
		$formData.topo_image = file;
	};
</script>

<SuperDebugHelper class="fixed bottom-6 right-6 z-40" />
<form method="post" class="flex flex-col gap-4" enctype="multipart/form-data" use:enhance>
	<Form.Field {form} name="sector_id">
		<Form.Control>
			{#snippet children({ props })}
				<Form.Label>{m.admin_routes_form_label_sector()}</Form.Label>
				<Select.Root class="w-full min-w-0" {...props} bind:value={$formData.sector_id} required onchange={handleSectorChange}>
					<Select.Option value="">{m.admin_routes_form_select_sector_placeholder()}</Select.Option>
					{#each sectors as sector (sector.id)}
						<Select.Option value={sector.id}>{sector.name}</Select.Option>
					{/each}
				</Select.Root>
			{/snippet}
		</Form.Control>
		<Form.Description>{m.admin_routes_form_description_sector()}</Form.Description>
		<Form.FieldErrors />
	</Form.Field>

	<Separator />

	<Form.Field {form} name="type" class="w-1/2">
		<Form.Control>
			{#snippet children({ props })}
				<Form.Label>{m.admin_routes_form_label_type()}</Form.Label>
				<Select.Root class="w-full min-w-0" {...props} bind:value={$formData.type} required disabled={formDisabled}>
					<Select.Option value="">{m.admin_routes_form_select_climbing_type_placeholder()}</Select.Option>
					{#each climbingTypes as climbing_type}
						<Select.Option value={climbing_type}>{climbing_type}</Select.Option>
					{/each}
				</Select.Root>
			{/snippet}
		</Form.Control>
		<Form.FieldErrors />
	</Form.Field>

	<Form.Field {form} name="name">
		<Form.Control>
			{#snippet children({ props })}
				<Form.Label>{m.admin_routes_form_label_name()}</Form.Label>
				<Input {...props} type="text" bind:value={$formData.name} required disabled={formDisabled} />
			{/snippet}
		</Form.Control>
		<Form.FieldErrors />
	</Form.Field>

	<Form.Field {form} name="description">
		<Form.Control>
			{#snippet children({ props })}
				<Form.Label>{m.admin_routes_form_label_description()}</Form.Label>
				<Textarea {...props} class="w-full" rows={4} bind:value={$formData.description as string | undefined} disabled={formDisabled}/>
			{/snippet}
		</Form.Control>
		<Form.FieldErrors />
	</Form.Field>

	<div class="flex gap-2">
		<Form.Field {form} name="height">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>{m.admin_routes_form_label_height()}</Form.Label>
					<Input {...props} type="number" bind:value={$formData.height as number | undefined} required disabled={formDisabled} />
				{/snippet}
			</Form.Control>
			<Form.FieldErrors />
		</Form.Field>

		<Form.Field {form} name="grade">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>{m.admin_routes_form_label_grade()}</Form.Label>
					<Input {...props} type="number" bind:value={$formData.grade as number | undefined} required disabled={formDisabled} />
				{/snippet}
			</Form.Control>
			<Form.FieldErrors />
		</Form.Field>

		<Form.Field {form} name="gear_count">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>{m.admin_routes_form_label_gear_count()}</Form.Label>
					<Input {...props} type="number" bind:value={$formData.gear_count as number | undefined} required disabled={formDisabled} />
				{/snippet}
			</Form.Control>
			<Form.FieldErrors />
		</Form.Field>
	</div>

	<Form.Field {form} name="bolter_name">
		<Form.Control>
			{#snippet children({ props })}
				<Form.Label>{m.admin_routes_form_label_bolter_name()}</Form.Label>
				<Input {...props} type="text" bind:value={$formData.bolter_name as string | undefined} required disabled={formDisabled} />
			{/snippet}
		</Form.Control>
		<Form.FieldErrors />
	</Form.Field>

	<Form.Field {form} name="first_ascent_climber_name">
		<Form.Control>
			{#snippet children({ props })}
				<Form.Label>{m.admin_routes_form_label_first_ascent()}</Form.Label>
				<Input {...props} type="text" bind:value={$formData.first_ascent_climber_name as string | undefined} required disabled={formDisabled} />
			{/snippet}
		</Form.Control>
		<Form.FieldErrors />
	</Form.Field>

	<Separator />

	<Tabs.Root value="sector_topo" class="flex w-full flex-col gap-2" disabled={formDisabled}>
		<Tabs.List class="my-4">
			<Tabs.Trigger value="sector_topo">{m.admin_routes_form_tab_sector_topo()}</Tabs.Trigger>
			<Tabs.Trigger value="route_topo">{m.admin_routes_form_tab_route_topo()}</Tabs.Trigger>
		</Tabs.List>
		<Tabs.Content value="sector_topo" class="w-full">
			<div class="flex flex-col gap-2">
				<Form.Field {form} name="sector_topo_image_id">
					<Form.Control>
						{#snippet children({ props })}
							<Form.Label>{m.admin_routes_form_label_sector_topo_image()}</Form.Label>
							<Select.Root class="w-full min-w-0" {...props} bind:value={$formData.sector_topo_image_id} required disabled={formDisabled}>
								<Select.Option value="">{m.admin_routes_form_select_sector_image_placeholder()}</Select.Option>
								{#each sectorImages as sectorImage (sectorImage.image_id)}
									<Select.Option value={sectorImage.image_id}
										>{`${sectorImage.order}${sectorImage.is_primary ? ` ${m.admin_routes_form_sector_image_primary_badge()}` : ''}`}</Select.Option
									>
								{/each}
							</Select.Root>
						{/snippet}
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
				<TopoEditor.Root class="min-h-[300px]">
					<TopoEditor.TopoImage src={selectedSectorTopoImage} />
					<TopoEditor.SvgOverlay
						bind:points={$formData.sector_topo_line_points}
						bind:output={$formData.sector_topo_image_overlay}
						debug={true}
					/>
				</TopoEditor.Root>
			</div>
		</Tabs.Content>
		<Tabs.Content value="route_topo" class="w-full">
			<div class="flex flex-col gap-2">
				<Form.Field {form} name="topo_image">
					<Form.Control>
						{#snippet children({ props })}
							<Form.Label>{m.admin_routes_form_label_route_topo_image_upload()}</Form.Label>
							<Input
								{...props}
								type="file"
								accept=".jpeg,.jpg,.png,.webp"
								class="h-auto"
								disabled={formDisabled}
								onchange={handleRouteTopoFileChange}
							/>
						{/snippet}
					</Form.Control>
					<Form.Description>
						{m.admin_routes_form_description_route_topo_image_upload()}
					</Form.Description>
					<Form.FieldErrors />
				</Form.Field>

				<TopoEditor.Root class="min-h-[300px]">
					<TopoEditor.TopoImage src={routeTopoSource} />
					<TopoEditor.SvgOverlay
						bind:points={$formData.topo_line_points}
						bind:output={$formData.topo_image_overlay}
						debug={true}
					/>
				</TopoEditor.Root>
			</div>
		</Tabs.Content>
	</Tabs.Root>
	<div class="mt-4 flex justify-end">
		<Button type="submit">{m.admin_routes_form_save()}</Button>
	</div>
</form>
