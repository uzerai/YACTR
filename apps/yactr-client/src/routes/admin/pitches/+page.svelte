<script lang="ts">
	import * as Table from '$lib/components/ui/table';
	import * as Card from '$lib/components/ui/card';
	import * as Empty from '$lib/components/ui/empty';
	import { Button } from '$lib/components/ui/button';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-6">
	<div class="flex items-center justify-between">
		<h1 class="text-4xl">Pitches</h1>
	</div>

	{#if data.pitches && data.pitches.length > 0}
		<Card.Root>
			<Card.Header>
				<Card.CardAction>
					<Button href="/admin/pitches/new" variant="default">Create Pitch</Button>
				</Card.CardAction>
			</Card.Header>
			<Card.Content>
				<Table.Root>
					<Table.Header>
						<Table.Row>
							<Table.Head>Id</Table.Head>
							<Table.Head>Name</Table.Head>
							<Table.Head>Type</Table.Head>
							<Table.Head>Sector Id</Table.Head>
							<Table.Head>Actions</Table.Head>
						</Table.Row>
					</Table.Header>
					<Table.Body>
						{#each data.pitches as pitch (pitch.id)}
							<Table.Row>
								<Table.Cell>{pitch.id}</Table.Cell>
								<Table.Cell>{pitch.name}</Table.Cell>
								<Table.Cell>{pitch.type}</Table.Cell>
								<Table.Cell>{pitch.sector_id}</Table.Cell>
								<Table.Cell class="flex gap-1">
									<Button href={`/admin/pitches/${pitch.id}`} variant="outline">Edit</Button>
									<form method="post" action="?/delete">
										<input type="hidden" name="pitch_id" value={pitch.id} />
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
			<Empty.Title>No pitches yet</Empty.Title>
			<Empty.Description>Create the first pitch to get started.</Empty.Description>
			<Empty.Content>
				<Button href="/admin/pitches/new">Create Pitch</Button>
			</Empty.Content>
		</Empty.Root>
	{/if}
</div>
