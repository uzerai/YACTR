<script lang="ts">
	import {
		Button,
		Heading,
		Hr,
		Input,
		Table,
		TableBody,
		TableBodyCell,
		TableBodyRow,
		TableHead,
		TableHeadCell
	} from 'flowbite-svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
</script>

<div class="flex items-center justify-between">
	<Heading tag="h1">Routes</Heading>
	<Button href="/admin/routes/new" color="primary">Create Route</Button>
</div>
<Hr />
{#if data.routes && data.routes.length > 0}
	<Table>
		<TableHead>
			<TableHeadCell>Id</TableHeadCell>
			<TableHeadCell>Name</TableHeadCell>
			<TableHeadCell>Sector</TableHeadCell>
			<TableHeadCell>Grade</TableHeadCell>
			<TableHeadCell>First Ascent</TableHeadCell>
			<TableHeadCell>Bolter</TableHeadCell>
			<TableHeadCell>Description</TableHeadCell>
			<TableHeadCell>Created At</TableHeadCell>
			<TableHeadCell>Updated At</TableHeadCell>
			<TableHeadCell>Actions</TableHeadCell>
		</TableHead>
		<TableBody>
			{#each data.routes as route}
				<TableBodyRow>
					<TableBodyCell>{route.id}</TableBodyCell>
					<TableBodyCell>{route.name}</TableBodyCell>
					<TableBodyCell>SectorId</TableBodyCell>
					<TableBodyCell>{route.grade}</TableBodyCell>
					<TableBodyCell>{route.first_ascent_date}</TableBodyCell>
					<TableBodyCell>{route.bolter_name}</TableBodyCell>
					<TableBodyCell>{route.description}</TableBodyCell>
					<TableBodyCell>CreatedAt</TableBodyCell>
					<TableBodyCell>UpdatedAt</TableBodyCell>
					<TableBodyCell class="flex gap-1">
						<Button href={`/admin/routes/${route.id}`} color="blue">Edit</Button>
						<form method="post" action="?/delete">
							<Input type="hidden" name="route_id" value={route.id} />
							<Button type="submit" color="red">Delete</Button>
						</form>
					</TableBodyCell>
				</TableBodyRow>
			{/each}
		</TableBody>
	</Table>
{:else}
	<p>No routes found</p>
{/if}
