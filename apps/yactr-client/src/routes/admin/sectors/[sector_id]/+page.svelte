<script lang="ts">
	import type { PageProps } from './$types';
	import SectorManagementForm from '../sector_management_form.svelte';
	import type { YactrEndpointsSectorsSectorRequestData } from '$lib/api';
	import { Heading, Hr, Indicator, Tooltip } from 'flowbite-svelte';

	let { data, params }: PageProps = $props();

	let sector = $derived({
		...data.sector,
		id: params.sector_id
	});
</script>

<Heading tag="h1" class="flex items-center gap-2"
	>{sector.name}<Indicator color="amber" /><Tooltip class="text-xs" placement="right"
		>You are editing this sector</Tooltip
	></Heading
>
<Hr />
<SectorManagementForm
	bind:sector
	areas={data.areas ?? []}
	loadedImages={sector.sector_images ?? []}
/>
