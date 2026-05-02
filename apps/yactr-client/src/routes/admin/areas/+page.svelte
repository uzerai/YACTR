<script lang="ts">
	import { getCoreRowModel, type ColumnDef, type PaginationState } from '@tanstack/table-core';
	import { createRawSnippet } from 'svelte';
	import {
		createSvelteTable,
		DataTableView,
		renderComponent,
		renderSnippet
	} from '$lib/components/ui/data-table';
	import DataTableFilters from '$lib/components/ui/data-table/data-table-filters.svelte';
	import type { ColumnFilterConfig } from '$lib/components/ui/data-table/filter-config';
	import { useTableUrlFilters } from '$lib/hooks/use-table-url-filters.svelte.js';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	import { Button } from '$lib/components/ui/button';
	import * as Empty from '$lib/components/ui/empty';
	import { m } from '$lib/paraglide/messages.js';
	import AreaTableActions from './area-table-actions.svelte';
	import type { GetAllAreasResponseItem } from '$lib/api';

	let { data }: PageProps = $props();

	const createdAtSnippet = createRawSnippet<[{ value?: string | null }]>((getValue) => {
		const { value } = getValue();
		return {
			render: () => (value ? m.common_iso_datetime({ date: value }) : '')
		};
	});

	const columns: ColumnDef<GetAllAreasResponseItem>[] = [
		{
			accessorKey: 'id',
			header: m.admin_areas_table_id()
		},
		{
			accessorKey: 'name',
			header: m.admin_areas_table_name()
		},
		{
			accessorKey: 'description',
			header: m.admin_areas_table_description()
		},
		{
			accessorKey: 'created_at',
			header: m.admin_areas_table_created_at(),
			cell: ({ row }) => renderSnippet(createdAtSnippet, { value: row.original.created_at })
		},
		{
			accessorKey: 'updated_at',
			header: m.admin_areas_table_updated_at(),
			cell: ({ row }) => renderSnippet(createdAtSnippet, { value: row.original.updated_at })
		},
		{
			id: 'actions',
			header: m.admin_areas_table_actions(),
			enableSorting: false,
			cell: ({ row }) => renderComponent(AreaTableActions, { areaId: row.original.id })
		}
	];

	const areaFilterConfig: ColumnFilterConfig<GetAllAreasResponseItem> = {
		name: {
			type: 'string',
			queryParameter: 'name',
			placeholder: m.admin_areas_table_name()
		},
		country_name: {
			type: 'string',
			queryParameter: 'country_name',
			placeholder: ''
		},
		created_at: {
			type: 'date',
			afterQueryParameter: 'created_after',
			beforeQueryParameter: 'created_before',
			label: m.admin_areas_table_created_at()
		}
	};

	const { filterValues, setFilterValue, applyPagination } = useTableUrlFilters({
		filterConfig: areaFilterConfig,
		getServerFilters: () => data.filters
	});

	function onFilterValueChange(queryParameter: string, value: string, debounce: boolean) {
		setFilterValue(queryParameter, value, debounce);
	}

	let pagination = $state<PaginationState>({ pageIndex: 0, pageSize: 10 });

	const table = createSvelteTable({
		get data() {
			return data.areas;
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
		manualFiltering: true,
		manualPagination: true
	});

	$effect(() => {
		pagination = {
			pageIndex: Math.max(0, data.pagination.page - 1),
			pageSize: data.pagination.page_size
		};
	});

	const hasActiveFilters = $derived(
		Boolean(
			data.filters.name ||
				data.filters.country_name ||
				data.filters.created_before ||
				data.filters.created_after
		)
	);
</script>

<div class="flex flex-col gap-6 p-4 max-w-7xl mx-auto">
	<div class="flex items-center justify-between">
		<h1 class="text-4xl">{m.admin_areas_title()}</h1>
	</div>

	{#if data.areas && (data.areas.length > 0 || hasActiveFilters)}
		<Card.Root>
			<Card.Header>
				<Card.CardAction>
					<Button href="/admin/areas/new" variant="default">{m.admin_areas_create_area()}</Button>
				</Card.CardAction>
			</Card.Header>
			<Card.Content>
				<DataTableView
					{table}
					emptyMessage={m.admin_areas_empty_title()}
				>
					{#snippet toolbar()}
						<DataTableFilters
							{columns}
							filterConfig={areaFilterConfig}
							values={filterValues}
							onValueChange={onFilterValueChange}
						/>
					{/snippet}
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
			<Empty.Title>{m.admin_areas_empty_title()}</Empty.Title>
			<Empty.Description>{m.admin_areas_empty_description()}</Empty.Description>
			<Empty.Content>
				<Button variant="default" href="/admin/areas/new">{m.admin_areas_create_area()}</Button>
			</Empty.Content>
		</Empty.Root>
	{/if}
</div>
