<script lang="ts">
	import * as Table from '$lib/components/ui/table';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	import { Button } from '$lib/components/ui/button';
	import * as Empty from '$lib/components/ui/empty';
	import { m } from '$lib/paraglide/messages.js';

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-6 p-4 max-w-7xl mx-auto">
	<div class="flex items-center justify-between">
		<h1 class="text-4xl">{m.admin_areas_title()}</h1>
	</div>
	
	{#if data.areas && data.areas.length > 0}
		<Card.Root>
			<Card.Header>
				<Card.CardAction>
					<Button href="/admin/areas/new" variant="default">{m.admin_areas_create_area()}</Button>
				</Card.CardAction>
			</Card.Header>
			<Card.Content>
				<Table.Root>
					<Table.Header>
						<Table.Row>
							<Table.Head>{m.admin_areas_table_id()}</Table.Head>
							<Table.Head>{m.admin_areas_table_name()}</Table.Head>
							<Table.Head>{m.admin_areas_table_description()}</Table.Head>
							<Table.Head>{m.admin_areas_table_created_at()}</Table.Head>
							<Table.Head>{m.admin_areas_table_updated_at()}</Table.Head>
							<Table.Head>{m.admin_areas_table_actions()}</Table.Head>
						</Table.Row>
					</Table.Header>
					<Table.Body>
						{#each data.areas as area (area.id)}
							<Table.Row>
								<Table.Cell>{area.id}</Table.Cell>
								<Table.Cell>{area.name}</Table.Cell>
								<Table.Cell>{area.description}</Table.Cell>
								<Table.Cell>
									{area.created_at ? m.common_iso_datetime({ date: area.created_at }) : ''}
								</Table.Cell>
								<Table.Cell>
									{area.updated_at ? m.common_iso_datetime({ date: area.updated_at }) : ''}
								</Table.Cell>
								<Table.Cell class="flex gap-1">
									<Button href={`/admin/areas/${area.id}`} variant="outline">{m.admin_areas_edit()}</Button>
									<form method="post" action="?/delete">
										<input type="hidden" name="area_id" value={area.id} />
										<Button type="submit" variant="destructive">{m.admin_areas_delete()}</Button>
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
			<Empty.Title>{m.admin_areas_empty_title()}</Empty.Title>
			<Empty.Description>{m.admin_areas_empty_description()}</Empty.Description>
			<Empty.Content>
				<Button variant="default" href="/admin/areas/new">{m.admin_areas_create_area()}</Button>
			</Empty.Content>
		</Empty.Root>
	{/if}
</div>
