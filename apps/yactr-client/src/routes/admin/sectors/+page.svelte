<script lang="ts">
	import {
		Table,
		TableBodyRow,
		TableHeadCell,
		TableBody,
		TableHead,
		TableBodyCell,
		Heading,
		Button,
		Hr,
		P,
		Input
	} from 'flowbite-svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
</script>

<div class="flex items-center justify-between">
	<Heading tag="h1">Sectors</Heading>
	<Button href="/admin/sectors/new" color="primary">Create Sector</Button>
</div>
<Hr />

{#if data.sectors && data.sectors.length > 0}
	<Table>
		<TableHead>
			<TableHeadCell>Id</TableHeadCell>
			<TableHeadCell>Name</TableHeadCell>
			<TableHeadCell>Area Id</TableHeadCell>
			<TableHeadCell>Created At</TableHeadCell>
			<TableHeadCell>Updated At</TableHeadCell>
			<TableHeadCell>Actions</TableHeadCell>
		</TableHead>
		<TableBody>
			{#each data.sectors as sector}
				<TableBodyRow>
					<TableBodyCell>{sector.id}</TableBodyCell>
					<TableBodyCell>{sector.name}</TableBodyCell>
					<TableBodyCell
						><a href={`/admin/areas/${sector.area_id}`} class="text-blue-500 underline"
							>{sector.area_id}</a
						></TableBodyCell
					>
					<TableBodyCell>{'createdat'}</TableBodyCell>
					<TableBodyCell>{'updatedat'}</TableBodyCell>
					<TableBodyCell class="flex gap-1">
						<Button href={`/admin/sectors/${sector.id}`} color="blue">Edit</Button>
						<form method="post" action="?/delete">
							<Input type="hidden" name="sector_id" value={sector.id} />
							<Button type="submit" color="red">Delete</Button>
						</form>
					</TableBodyCell>
				</TableBodyRow>
			{/each}
		</TableBody>
	</Table>
{:else}
	<P>No sectors found</P>
{/if}
