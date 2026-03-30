<script lang="ts">
	import { Point as PointInput, MultiPolygon as MultiPolygonInput } from '$lib/components/geo-input';
	import { z } from 'zod';
	import { untrack } from 'svelte';
	import { m } from '$lib/paraglide/messages.js';
	import { superForm, type SuperValidated } from 'sveltekit-superforms';
	import { zAreaRequestData } from '$lib/api/generated/zod.gen';
	import * as Form from '$lib/components/ui/form';
	import { Input } from '$lib/components/ui/input';
	import { Textarea } from '$lib/components/ui/textarea';
	import { Button } from '$lib/components/ui/button';
	import * as Tooltip from '$lib/components/ui/tooltip';
	import { HugeiconsIcon as Icon } from '@hugeicons/svelte';
	import { HelpCircleIcon } from '@hugeicons/core-free-icons';
	import { SuperDebugHelper, useSuperDebugForm } from '$lib/components/forms/util/super-debug-helper';
	
	let { data }: { data: SuperValidated<z.infer<typeof zAreaRequestData>> } = $props();

	const form = superForm(untrack(() => data), {
		dataType: 'json'
	});

	const { form: formData, enhance } = form;
	useSuperDebugForm(formData);
</script>

<SuperDebugHelper class="fixed bottom-6 right-6 z-40" />

<form method="post" use:enhance>
	<div class="grid gap-6">
		<Form.Field {form} name="name">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>{m.admin_areas_form_label_name()}</Form.Label>
					<Input {...props} type="string" bind:value={$formData.name} />
				{/snippet}
			</Form.Control>
			<Form.FieldErrors />
		</Form.Field>

		<Form.Field {form} name="description">
			<Form.Control>
				{#snippet children({ props })}
					<Form.Label>{m.admin_areas_form_label_description()}</Form.Label>
					<Textarea {...props} class="w-full" rows={4} bind:value={$formData.description} />
				{/snippet}
			</Form.Control>
			<Form.FieldErrors />
		</Form.Field>
	
		<div class="flex flex-col gap-4 md:flex-row">
			<Form.Field {form} name="location" class="w-full flex flex-col gap-2 md:w-1/2">
				<Form.Control>
					<span class="flex gap-4">
						<Form.Label for="location">{m.admin_areas_form_label_location()}</Form.Label>
						<Tooltip.Root>
							<Tooltip.Trigger>
								<Icon icon={HelpCircleIcon} class="size-4 shrink-0" />
							</Tooltip.Trigger>
							<Tooltip.Content>
								<p>{m.admin_areas_form_tooltip_location()}</p>
							</Tooltip.Content>
						</Tooltip.Root>
					</span>
					<div class="h-[50dvh]">
						<PointInput bind:location={$formData.location} mapCenter={$formData.location?.coordinates} />
					</div>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>
			<Form.Field {form} name="boundary" class="flex w-full flex-col gap-2 md:w-1/2">
				<Form.Control>
					<span class="flex gap-4">
						<Form.Label for="boundary">{m.admin_areas_form_label_boundary()}</Form.Label>
						<Tooltip.Root>
							<Tooltip.Trigger>
								<Icon icon={HelpCircleIcon} class="size-4 shrink-0" />
							</Tooltip.Trigger>
							<Tooltip.Content>
								<p>{m.admin_areas_form_tooltip_boundary()}</p>
							</Tooltip.Content>
						</Tooltip.Root>
					</span>
					<div class="h-[50dvh]">
						<MultiPolygonInput bind:boundary={$formData.boundary} mapCenter={$formData.location?.coordinates} />
					</div>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>
		</div>
	</div>
	<div class="mt-4 flex justify-end">
		<Button type="submit" color="primary">{m.admin_areas_form_save()}</Button>
	</div>
</form>
