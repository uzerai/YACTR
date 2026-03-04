<script lang="ts">
	import {
		Table,
		TableBodyRow,
		TableHeadCell,
		TableBody,
		TableHead,
		TableBodyCell
	} from 'flowbite-svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-4">
	<div class="flex justify-between">
		<h1 class="text-2xl font-bold text-slate-900">Organizations</h1>
		<div class="flex">
			<a
				href="/admin/organizations/new"
				class="rounded-md bg-fire-engine-red px-4 py-2 text-antiflash-white">Create Organization</a
			>
		</div>
	</div>
	<hr />

	{#if data.organizations && data.organizations.length > 0}
		<Table>
			<TableHead>
				<TableHeadCell>Id</TableHeadCell>
				<TableHeadCell>Name</TableHeadCell>
				<TableHeadCell>Created At</TableHeadCell>
				<TableHeadCell>Updated At</TableHeadCell>
			</TableHead>
			<TableBody>
				{#each data.organizations as org}
					<TableBodyRow>
						<TableBodyCell>{org.id}</TableBodyCell>
						<TableBodyCell>{org.name}</TableBodyCell>
						<TableBodyCell>{org.created_at}</TableBodyCell>
						<TableBodyCell>{org.updated_at}</TableBodyCell>
					</TableBodyRow>
				{/each}
			</TableBody>
		</Table>
	{:else}
		<p>No organizations found</p>
	{/if}
</div>
