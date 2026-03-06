<script lang="ts">
	import type { PageProps } from './$types';
	import AreaManagementForm from '../area_management_form.svelte';
	import { Heading, Hr, Indicator, Tooltip } from 'flowbite-svelte';

	let { data }: PageProps = $props();
</script>

<Heading tag="h1" class="flex items-center gap-2"
	>{data.form.data.name}<Indicator color="amber" /><Tooltip class="text-xs" placement="right"
		>You are editing this area</Tooltip
	></Heading
>
<Hr />
<AreaManagementForm data={data.form} />
