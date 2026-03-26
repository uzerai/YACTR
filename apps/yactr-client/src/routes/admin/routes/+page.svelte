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
		<h1 class="text-4xl">{m.admin_routes_title()}</h1>
	</div>

	{#if data.routes && data.routes.length > 0}
		<Card.Root>
			<Card.Header>
				<Card.CardAction>
					<Button href="/admin/routes/new" variant="default">{m.admin_routes_create_route()}</Button>
				</Card.CardAction>
			</Card.Header>
			<Card.Content>
				<Table.Root>
					<Table.Header>
						<Table.Row>
							<Table.Head>{m.admin_routes_table_name()}</Table.Head>
							<Table.Head>{m.admin_routes_table_sector()}</Table.Head>
							<Table.Head>{m.admin_routes_table_grade()}</Table.Head>
							<Table.Head>{m.admin_routes_table_first_ascent_by()}</Table.Head>
							<Table.Head>{m.admin_routes_table_bolter()}</Table.Head>
							<Table.Head>{m.admin_routes_table_description()}</Table.Head>
							<Table.Head>{m.admin_routes_table_created_at()}</Table.Head>
							<Table.Head>{m.admin_routes_table_updated_at()}</Table.Head>
							<Table.Head>{m.admin_routes_table_actions()}</Table.Head>
						</Table.Row>
					</Table.Header>
					<Table.Body>
						{#each data.routes as route (route.id)}
							<Table.Row>
								<Table.Cell>{route.name}</Table.Cell>
								<Table.Cell>{route.sector_id}</Table.Cell>
								<Table.Cell>{route.grade}</Table.Cell>
								<Table.Cell>{route.first_ascent_climber_name}</Table.Cell>
								<Table.Cell>{route.bolter_name}</Table.Cell>
								<Table.Cell>{route.description}</Table.Cell>
								<Table.Cell>
									{route.created_at ? m.common_iso_datetime({ date: route.created_at }) : ''}
								</Table.Cell>
								<Table.Cell>
									{route.updated_at ? m.common_iso_datetime({ date: route.updated_at }) : ''}
								</Table.Cell>
								<Table.Cell class="flex gap-1">
									<Button href={`/admin/routes/${route.id}`} variant="outline">{m.admin_routes_edit()}</Button>
									<form method="post" action="?/delete">
										<input type="hidden" name="route_id" value={route.id} />
										<Button type="submit" variant="destructive">{m.admin_routes_delete()}</Button>
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
			<Empty.Title>{m.admin_routes_empty_title()}</Empty.Title>
			<Empty.Description>{m.admin_routes_empty_description()}</Empty.Description>
			<Empty.Content>
				<Button variant="default" href="/admin/routes/new">{m.admin_routes_create_route()}</Button>
			</Empty.Content>
		</Empty.Root>
	{/if}
</div>
