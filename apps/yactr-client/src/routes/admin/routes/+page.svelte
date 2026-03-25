<script lang="ts">
	import * as Table from '$lib/components/ui/table';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	import { Button } from '$lib/components/ui/button';
	import * as Empty from '$lib/components/ui/empty';

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-6 p-4 max-w-7xl mx-auto">
	<div class="flex items-center justify-between">
		<h1 class="text-4xl">Routes</h1>
	</div>

	{#if data.routes && data.routes.length > 0}
		<Card.Root>
			<Card.Header>
				<Card.CardAction>
					<Button href="/admin/routes/new" variant="default">Create Route</Button>
				</Card.CardAction>
			</Card.Header>
			<Card.Content>
				<Table.Root>
					<Table.Header>
						<Table.Row>
							<Table.Head>Id</Table.Head>
							<Table.Head>Name</Table.Head>
							<Table.Head>Sector</Table.Head>
							<Table.Head>Grade</Table.Head>
							<Table.Head>First Ascent</Table.Head>
							<Table.Head>Bolter</Table.Head>
							<Table.Head>Description</Table.Head>
							<Table.Head>Created At</Table.Head>
							<Table.Head>Updated At</Table.Head>
							<Table.Head>Actions</Table.Head>
						</Table.Row>
					</Table.Header>
					<Table.Body>
						{#each data.routes as route (route.id)}
							<Table.Row>
								<Table.Cell>{route.id}</Table.Cell>
								<Table.Cell>{route.name}</Table.Cell>
								<Table.Cell>SectorId</Table.Cell>
								<Table.Cell>{route.grade}</Table.Cell>
								<Table.Cell>{route.first_ascent_date}</Table.Cell>
								<Table.Cell>{route.bolter_name}</Table.Cell>
								<Table.Cell>{route.description}</Table.Cell>
								<Table.Cell>CreatedAt</Table.Cell>
								<Table.Cell>UpdatedAt</Table.Cell>
								<Table.Cell class="flex gap-1">
									<Button href={`/admin/routes/${route.id}`} variant="outline">Edit</Button>
									<form method="post" action="?/delete">
										<input type="hidden" name="route_id" value={route.id} />
										<Button type="submit" variant="destructive">Delete</Button>
									</form>
								</Table.Cell>
							</Table.Row>
						{/each}
					</Table.Body>
				</Table.Root>
			</Card.Content>
		</Card.Root>
	{:else}
		<Empty.Root>
			<Empty.Title>No routes found</Empty.Title>
			<Empty.Description>Add a route to see it listed here.</Empty.Description>
			<Empty.Content>
				<Button variant="default" href="/admin/routes/new">Create Route</Button>
			</Empty.Content>
		</Empty.Root>
	{/if}
</div>
