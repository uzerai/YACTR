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
		<h1 class="text-2xl font-bold text-slate-900">Ascents</h1>
		<div class="flex">
			<a
				href="/admin/ascents/new"
				class="rounded-md bg-fire-engine-red px-4 py-2 text-antiflash-white">Create Ascent</a
			>
		</div>
	</div>
	<hr />

	{#if data.ascents && data.ascents.length > 0}
		<Table>
			<TableHead>
				<TableHeadCell>Id</TableHeadCell>
				<TableHeadCell>User</TableHeadCell>
				<TableHeadCell>Route</TableHeadCell>
				<TableHeadCell>Type</TableHeadCell>
				<TableHeadCell>Completed At</TableHeadCell>
				<TableHeadCell>Actions</TableHeadCell>
			</TableHead>
			<TableBody>
				{#each data.ascents as ascent}
					<TableBodyRow>
						<TableBodyCell>{ascent.id}</TableBodyCell>
						<TableBodyCell>{ascent.user_id}</TableBodyCell>
						<TableBodyCell>{ascent.route?.name}</TableBodyCell>
						<TableBodyCell>{ascent.type}</TableBodyCell>
						<TableBodyCell>{ascent.completed_at}</TableBodyCell>
						<TableBodyCell class="flex gap-1">
							<form method="post" action="?/delete">
								<input type="hidden" name="ascent_id" value={ascent.id} />
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
		<p>No ascents found</p>
	{/if}
</div>
