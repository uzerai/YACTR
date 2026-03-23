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
		<h1 class="text-4xl">Areas</h1>
	</div>
	
	{#if data.areas && data.areas.length > 0}
		<Card.Root>
			<Card.Header>
				<Card.CardAction>
					<Button href="/admin/areas/new" variant="default">Create Area</Button>
				</Card.CardAction>
			</Card.Header>
			<Card.Content>
				<Table.Root>
					<Table.Header>
						<Table.Row>
							<Table.Head>Id</Table.Head>
							<Table.Head>Name</Table.Head>
							<Table.Head>Description</Table.Head>
							<Table.Head>Created At</Table.Head>
							<Table.Head>Updated At</Table.Head>
							<Table.Head>Actions</Table.Head>
						</Table.Row>
					</Table.Header>
					<Table.Body>
						{#each data.areas as area (area.id)}
							<Table.Row>
								<Table.Cell>{area.id}</Table.Cell>
								<Table.Cell>{area.name}</Table.Cell>
								<Table.Cell>{area.description}</Table.Cell>
								<Table.Cell>{area.created_at}</Table.Cell>
								<Table.Cell>{area.updated_at}</Table.Cell>
								<Table.Cell class="flex gap-1">
									<Button href={`/admin/areas/${area.id}`} variant="outline">Edit</Button>
									<form method="post" action="?/delete">
										<input type="hidden" name="area_id" value={area.id} />
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
			<Empty.Title>No areas found</Empty.Title>
			<Empty.Description>Create an area to get started</Empty.Description>
			<Empty.Content>
				<Button variant="default" href="/admin/areas/new">Create Area</Button>
			</Empty.Content>
		</Empty.Root>
	{/if}
</div>
