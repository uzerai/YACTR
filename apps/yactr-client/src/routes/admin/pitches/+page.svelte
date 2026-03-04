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
		<h1 class="text-2xl font-bold text-slate-900">Pitches</h1>
		<div class="flex">
			<a
				href="/admin/pitches/new"
				class="rounded-md bg-fire-engine-red px-4 py-2 text-antiflash-white">Create Pitch</a
			>
		</div>
	</div>
	<hr />

	{#if data.pitches && data.pitches.length > 0}
		<Table>
			<TableHead>
				<TableHeadCell>Id</TableHeadCell>
				<TableHeadCell>Name</TableHeadCell>
				<TableHeadCell>Type</TableHeadCell>
				<TableHeadCell>Sector Id</TableHeadCell>
				<TableHeadCell>Description</TableHeadCell>
				<TableHeadCell>Created At</TableHeadCell>
				<TableHeadCell>Updated At</TableHeadCell>
				<TableHeadCell>Actions</TableHeadCell>
			</TableHead>
			<TableBody>
				{#each data.pitches as pitch}
					<TableBodyRow>
						<TableBodyCell>{pitch.id}</TableBodyCell>
						<TableBodyCell>{pitch.name}</TableBodyCell>
						<TableBodyCell>{pitch.type}</TableBodyCell>
						<TableBodyCell>{pitch.sector_id}</TableBodyCell>
						<TableBodyCell>{pitch.description}</TableBodyCell>
						<TableBodyCell>{pitch.created_at}</TableBodyCell>
						<TableBodyCell>{pitch.updated_at}</TableBodyCell>
						<TableBodyCell class="flex gap-1">
							<form method="post" action="?/delete">
								<input type="hidden" name="pitch_id" value={pitch.id} />
								<button
									type="submit"
									class="cursor-pointer rounded-md bg-fire-engine-red px-4 py-2 text-antiflash-white"
									>Delete</button
								>
							</form>
						</TableBodyCell>
					</TableBodyRow>
				{/each}
			</TableBody>
		</Table>
	{:else}
		<p>No pitches found</p>
	{/if}
</div>
