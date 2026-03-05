<script lang="ts">
	import type { PageProps } from './$types';
	import AreaManagementForm from '../area_management_form.svelte';
	import type { YactrApiEndpointsAreasAreaRequestData } from '$lib/api';
	import { Heading, Hr, Indicator, Tooltip } from 'flowbite-svelte';

	let { data, params }: PageProps = $props();

	let area = $derived({
		...data.area,
		id: params.area_id
	}) as YactrApiEndpointsAreasAreaRequestData;
</script>

<Heading tag="h1" class="flex items-center gap-2"
	>{area.name}<Indicator color="amber" /><Tooltip class="text-xs" placement="right"
		>You are editing this area</Tooltip
	></Heading
>
<Hr />
<AreaManagementForm bind:area />
