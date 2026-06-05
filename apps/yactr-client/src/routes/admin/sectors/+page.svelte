<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import { getCoreRowModel, type ColumnDef, type PaginationState } from '@tanstack/table-core';
	import { createRawSnippet } from 'svelte';
	import { createSvelteTable, DataTableView, renderComponent, renderSnippet } from '$lib/components/ui/data-table';
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

	let pagination = $state<PaginationState>({ pageIndex: 0, pageSize: 10 });

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
				return pagination;
			}
		},
		onPaginationChange: (updater) => {
			const nextPagination = typeof updater === 'function' ? updater(pagination) : updater;
			pagination = nextPagination;
			applyPagination(nextPagination);
		},
		manualPagination: true
	});

	function applyPagination(nextPagination: PaginationState) {
		const url = new URL(page.url);
		url.searchParams.set('page', String(nextPagination.pageIndex + 1));
		url.searchParams.set('page_size', String(nextPagination.pageSize));

		if (url.search === page.url.search) return;

		const search = url.searchParams.toString();
		const target = search ? `${url.pathname}?${search}` : url.pathname;

		void goto(target, {
			replaceState: true,
			noScroll: true,
			keepFocus: true,
			invalidateAll: true
		});
	}

	$effect(() => {
		pagination = {
			pageIndex: Math.max(0, data.pagination.page - 1),
			pageSize: data.pagination.page_size
		};
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
