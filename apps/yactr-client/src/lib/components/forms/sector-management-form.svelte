<script lang="ts">
	import type { AreaResponse } from '$lib/api';
	import { Point as PointInput, Polygon as PolygonInput, LineString as LineStringInput } from '$lib/components/geo-input';
	import type { infer as ZodInfer } from 'zod';
	import { untrack } from 'svelte';
	import { m } from '$lib/paraglide/messages.js';
	import SuperDebug, { fieldProxy, superForm, type SuperValidated } from 'sveltekit-superforms';
	import { sectorManagementFormDto } from '$lib/components/forms';
	import SectorImagesManager from '$lib/components/orderable-image-upload/orderable-image-upload.svelte';
	import * as Form from '$lib/components/ui/form';
	import { Input } from '$lib/components/ui/input';
	import { Button } from '$lib/components/ui/button';
	import * as Tabs from '$lib/components/ui/tabs';
	import * as NativeSelect from '$lib/components/ui/native-select';
	import { Separator } from '$lib/components/ui/separator';

	let {
		data,
		areas = [] as AreaResponse[]
	}: {
		data: SuperValidated<ZodInfer<typeof sectorManagementFormDto>>;
		areas?: AreaResponse[];
	} = $props();

	const form = superForm(untrack(() => data), {
		dataType: 'json'
	});

	const { form: formData, enhance, errors, allErrors, message } = form;
	const formDisabled = $derived(!$formData.area_id);
	
	let derived_map_center = $derived(
		areas.find((area) => area.id === $formData.area_id)?.location?.coordinates
	);

	let sector_images = fieldProxy(form, 'sector_images');
</script>

<SuperDebug data={{ form: $formData, errors: $errors, allErrors: $allErrors, message: $message }} />
<form method="post" enctype="multipart/form-data" use:enhance>
	<div class="grid gap-6">
		<Form.Field {form} name="area_id">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>{m.admin_sectors_form_label_area()}</Form.Label>
					<NativeSelect.Root class="w-full min-w-0" {...props} bind:value={$formData.area_id} required>
						<NativeSelect.Option value="">{m.admin_sectors_form_select_area_placeholder()}</NativeSelect.Option>
						{#each areas as area (area.id)}
							<NativeSelect.Option value={area.id}>{area.name} ({area.id})</NativeSelect.Option>
						{/each}
					</NativeSelect.Root>
				{/snippet}
			</Form.Control>
			<Form.Description>{m.admin_sectors_form_description_area()}</Form.Description>
			<Form.FieldErrors />
		</Form.Field>

		<Form.Field {form} name="name">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>{m.admin_sectors_form_label_name()}</Form.Label>
					<Input {...props} type="text" bind:value={$formData.name} required disabled={formDisabled} />
				{/snippet}
			</Form.Control>
			<Form.FieldErrors />
		</Form.Field>

		<Separator />

		<Tabs.Root value="polygon" class="flex w-full flex-col gap-2" disabled={formDisabled}>
			<Tabs.List class="flex w-full min-w-0 max-w-full flex-wrap justify-start" variant="line">
				<Tabs.Trigger value="polygon">{m.admin_sectors_form_tab_polygon()}</Tabs.Trigger>
				<Tabs.Trigger value="entry">{m.admin_sectors_form_tab_entry()}</Tabs.Trigger>
				<Tabs.Trigger value="parking">{m.admin_sectors_form_tab_parking()}</Tabs.Trigger>
				<Tabs.Trigger value="approach">{m.admin_sectors_form_tab_approach()}</Tabs.Trigger>
			</Tabs.List>
			<Tabs.Content value="polygon" class="w-full">
				<Form.Field {form} name="sector_area" class="w-full">
					<Form.Control>
						{#snippet children()}
							<div class="h-[50dvh]">
								<PolygonInput
									bind:boundary={$formData.sector_area}
									disabled={formDisabled}
									mapCenter={derived_map_center}
								/>
							</div>
						{/snippet}
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			</Tabs.Content>

			<Tabs.Content value="entry" class="w-full">
				<Form.Field {form} name="entry_point" class="w-full">
					<Form.Control>
						{#snippet children()}
							<div class="h-[50dvh]">
								<PointInput
									bind:location={$formData.entry_point}
									disabled={formDisabled}
									mapCenter={derived_map_center}
								/>
							</div>
						{/snippet}
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			</Tabs.Content>

			<Tabs.Content value="parking" class="w-full">
				<Form.Field {form} name="recommended_parking_location" class="w-full">
					<Form.Control>
						{#snippet children()}
							<div class="h-[50dvh]">
								<PointInput
									bind:location={$formData.recommended_parking_location}
									disabled={formDisabled}
									mapCenter={derived_map_center}
								/>
							</div>
						{/snippet}
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			</Tabs.Content>

			<Tabs.Content value="approach" class="w-full">
				<Form.Field {form} name="approach_path" class="w-full">
					<Form.Control>
						{#snippet children()}
							<div class="h-[50dvh]">
								<LineStringInput
									bind:line={$formData.approach_path}
									disabled={formDisabled}
									mapCenter={derived_map_center}
								/>
							</div>
						{/snippet}
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			</Tabs.Content>
		</Tabs.Root>

		<Separator />

		<div class="flex flex-col gap-2">
			<h2 class="text-lg font-semibold">{m.admin_sectors_form_images_heading()}</h2>
			<SectorImagesManager bind:images={$sector_images} disabled={formDisabled} />
		</div>

		<div class="mt-4 flex justify-end">
			<Button type="submit" variant="default" disabled={formDisabled}>{m.admin_sectors_form_save()}</Button>
		</div>
	</div>
</form>
