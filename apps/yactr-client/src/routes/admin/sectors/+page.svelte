<script lang="ts">
	import { getCoreRowModel, type ColumnDef } from '@tanstack/table-core';
	import { createRawSnippet } from 'svelte';
	import {
		createSvelteTable,
		DataTableView,
		renderComponent,
		renderSnippet,
		useManualTableParams
	} from '$lib/components/ui/data-table';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	import { Button } from '$lib/components/ui/button';
	import * as Empty from '$lib/components/ui/empty';
	import { m } from '$lib/paraglide/messages.js';
	import type { GetAllSectorsResponseItem } from '$lib/api';
	import SectorAreaLink from './sector-area-link.svelte';
	import SectorTableActions from './sector-table-actions.svelte';

	let { data }: PageProps = $props();

	const createdAtSnippet = createRawSnippet<[{ value?: string | null }]>((getValue) => {
		const { value } = getValue();
		return {
			render: () => (value ? m.common_iso_datetime({ date: value }) : '')
		};
	});

	const columns: ColumnDef<GetAllSectorsResponseItem>[] = [
		{
			accessorKey: 'id',
			header: m.admin_sectors_table_id()
		},
		{
			accessorKey: 'name',
			header: m.admin_sectors_table_name()
		},
		{
			accessorKey: 'area_id',
			header: m.admin_sectors_table_area_id(),
			cell: ({ row }) => renderComponent(SectorAreaLink, { areaId: row.original.area_id })
		},
		{
			accessorKey: 'created_at',
			header: m.admin_sectors_table_created_at(),
			cell: ({ row }) => renderSnippet(createdAtSnippet, { value: row.original.created_at })
		},
		{
			accessorKey: 'updated_at',
			header: m.admin_sectors_table_updated_at(),
			cell: ({ row }) => renderSnippet(createdAtSnippet, { value: row.original.updated_at })
		},
		{
			id: 'actions',
			header: m.admin_sectors_table_actions(),
			enableSorting: false,
			cell: ({ row }) => renderComponent(SectorTableActions, { sectorId: row.original.id })
		}
	];

	const tableParams = useManualTableParams({
		getPagination: () => ({ page: data.pagination.page, pageSize: data.pagination.page_size })
	});

	const table = createSvelteTable({
		get data() {
			return data.sectors ?? [];
		},
		columns,
		getCoreRowModel: getCoreRowModel(),
		get rowCount() {
			return data.pagination.total_count;
		},
		state: {
			get pagination() {
				return tableParams.pagination;
			}
		},
		onPaginationChange: tableParams.onPaginationChange,
		manualPagination: true
	});
</script>

<div class="flex flex-col gap-6">
	<div class="flex items-center justify-between">
		<h1 class="text-4xl">{m.admin_sectors_title()}</h1>
	</div>

	{#if data.pagination.total_count > 0}
		<Card.Root>
			<Card.Header>
				<Card.CardAction>
					<Button href="/admin/sectors/new" variant="default">{m.admin_sectors_create_sector()}</Button>
				</Card.CardAction>
			</Card.Header>
			<Card.Content>
				<DataTableView {table} emptyMessage={m.admin_sectors_empty_title()}>
					{#snippet paginationSummary()}
						<p class="text-sm text-muted-foreground">
							{data.pagination.page} / {data.pagination.page_count} · {data.pagination.total_count}
						</p>
					{/snippet}
				</DataTableView>
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
