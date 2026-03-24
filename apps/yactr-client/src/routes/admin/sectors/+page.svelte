<script lang="ts">
	import * as Table from '$lib/components/ui/table';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	import { Button } from '$lib/components/ui/button';
	import * as Empty from '$lib/components/ui/empty';
	import { m } from '$lib/paraglide/messages.js';

	let { data }: PageProps = $props();
</script>

<div class="mx-auto flex max-w-7xl flex-col gap-6 p-4">
	<div class="flex items-center justify-between">
		<h1 class="text-4xl">{m.admin_sectors_title()}</h1>
	</div>

	{#if data.sectors && data.sectors.length > 0}
		<Card.Root>
			<Card.Header>
				<Card.CardAction>
					<Button href="/admin/sectors/new" variant="default">{m.admin_sectors_create_sector()}</Button>
				</Card.CardAction>
			</Card.Header>
			<Card.Content>
				<Table.Root>
					<Table.Header>
						<Table.Row>
							<Table.Head>{m.admin_sectors_table_id()}</Table.Head>
							<Table.Head>{m.admin_sectors_table_name()}</Table.Head>
							<Table.Head>{m.admin_sectors_table_area_id()}</Table.Head>
							<Table.Head>{m.admin_sectors_table_created_at()}</Table.Head>
							<Table.Head>{m.admin_sectors_table_updated_at()}</Table.Head>
							<Table.Head>{m.admin_sectors_table_actions()}</Table.Head>
						</Table.Row>
					</Table.Header>
					<Table.Body>
						{#each data.sectors as sector (sector.id)}
							<Table.Row>
								<Table.Cell>{sector.id}</Table.Cell>
								<Table.Cell>{sector.name}</Table.Cell>
								<Table.Cell>
									<a href={`/admin/areas/${sector.area_id}`} class="text-primary underline underline-offset-2">
										{sector.area_id}
									</a>
								</Table.Cell>
								<Table.Cell>
									{sector.created_at ? m.common_iso_datetime({ date: sector.created_at }) : ''}
								</Table.Cell>
								<Table.Cell>
									{sector.updated_at ? m.common_iso_datetime({ date: sector.updated_at }) : ''}
								</Table.Cell>
								<Table.Cell class="flex gap-1">
									<Button href={`/admin/sectors/${sector.id}`} variant="outline">{m.admin_sectors_edit()}</Button>
									<form method="post" action="?/delete">
										<input type="hidden" name="sector_id" value={sector.id} />
										<Button type="submit" variant="destructive">{m.admin_sectors_delete()}</Button>
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
			<Empty.Title>{m.admin_sectors_empty_title()}</Empty.Title>
			<Empty.Description>{m.admin_sectors_empty_description()}</Empty.Description>
			<Empty.Content>
				<Button variant="default" href="/admin/sectors/new">{m.admin_sectors_create_sector()}</Button>
			</Empty.Content>
		</Empty.Root>
	{/if}
</div>
