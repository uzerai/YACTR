<script lang="ts">
	import type { PageProps } from './$types';
	import SectorManagementForm from '../sector_management_form.svelte';
	import { Heading, Hr, Indicator, Tooltip } from 'flowbite-svelte';

	let { data }: PageProps = $props();
</script>

<Heading tag="h1" class="flex items-center gap-2">
	{data.sector.name}
	<Indicator color="amber" /><Tooltip class="text-xs" placement="right"
		>You are editing this sector</Tooltip>
	</Heading>
<Hr />
<SectorManagementForm
	data={data.form}
	areas={data.areas ?? []}
/>
