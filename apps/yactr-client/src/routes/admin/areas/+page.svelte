<script lang="ts">
	import {
		Table,
		TableBodyRow,
		TableHeadCell,
		TableBody,
		TableHead,
		TableBodyCell,
		Heading,
		Hr,
		Button,
		Input
	} from 'flowbite-svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
</script>

<div class="flex items-center justify-between">
	<Heading tag="h1">Areas</Heading>
	<Button href="/admin/areas/new" color="primary">Create Area</Button>
</div>
<Hr />
{#if data.areas && data.areas.length > 0}
	<Table>
		<TableHead>
			<TableHeadCell>Id</TableHeadCell>
			<TableHeadCell>Name</TableHeadCell>
			<TableHeadCell>Description</TableHeadCell>
			<TableHeadCell>Created At</TableHeadCell>
			<TableHeadCell>Updated At</TableHeadCell>
			<TableHeadCell>Actions</TableHeadCell>
		</TableHead>
		<TableBody>
			{#each data.areas as area}
				<TableBodyRow>
					<TableBodyCell>{area.id}</TableBodyCell>
					<TableBodyCell>{area.name}</TableBodyCell>
					<TableBodyCell>{area.description}</TableBodyCell>
					<TableBodyCell>{area.created_at}</TableBodyCell>
					<TableBodyCell>{area.updated_at}</TableBodyCell>
					<TableBodyCell class="flex gap-1">
						<Button href={`/admin/areas/${area.id}`} color="blue">Edit</Button>
						<form method="post" action="?/delete">
							<Input type="hidden" name="area_id" value={area.id} />
							<Button type="submit" color="red">Delete</Button>
						</form>
					</TableBodyCell>
				</TableBodyRow>
			{/each}
		</TableBody>
	</Table>
{:else}
	<p>No areas found</p>
{/if}
