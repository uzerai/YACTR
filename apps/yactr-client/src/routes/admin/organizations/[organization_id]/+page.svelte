<script lang="ts">
	import type { PageProps } from './$types';
	let { data, params }: PageProps = $props();
</script>

<div class="flex flex-col gap-4">
	<h1 class="text-2xl font-bold">Organization ({params.organization_id})</h1>
	{#if data.organization}
		<div class="flex flex-col gap-1">
			<div><span class="font-semibold">Id:</span> {data.organization.id}</div>
			<div><span class="font-semibold">Name:</span> {data.organization.name}</div>
			<div><span class="font-semibold">Created At:</span> {data.organization.created_at}</div>
			<div><span class="font-semibold">Updated At:</span> {data.organization.updated_at}</div>
		</div>
	{:else}
		<p>Not found</p>
	{/if}
</div>
