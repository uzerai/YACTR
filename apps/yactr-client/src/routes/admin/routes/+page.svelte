<script lang="ts">
	import { getCoreRowModel, type ColumnDef } from '@tanstack/table-core';
	import { createRawSnippet } from 'svelte';
	import {
		createSvelteTable,
		DataTableSortHeader,
		DataTableView,
		renderComponent,
		renderSnippet,
		useManualTableParams
	} from '$lib/components/ui/data-table';
	// import DataTableFilters from '$lib/components/ui/data-table/data-table-filters.svelte';
	// import type { ColumnFilterConfig } from '$lib/components/ui/data-table/filter-config';
	// import { useTableUrlFilters } from '$lib/hooks/use-table-url-filters.svelte';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	import { Button } from '$lib/components/ui/button';
	import * as Empty from '$lib/components/ui/empty';
	import { m } from '$lib/paraglide/messages.js';
	import RouteTableActions from './route-table-actions.svelte';
	import type { GetAllRoutesResponseItem } from '$lib/api';

	let { data }: PageProps = $props();

	const createdAtSnippet = createRawSnippet<[{ value?: string | null }]>((getValue) => {
		const { value } = getValue();
		return {
			render: () => (value ? m.common_iso_datetime({ date: value }) : '')
		};
	});

	// Sortable column ids must match the server's GetAllRoutesSortBy enum values
	// ('name', 'grade', 'created_at', 'updated_at', 'in_sector_order') as they are
	// passed through as the sort_by query parameter.
	const columns: ColumnDef<GetAllRoutesResponseItem>[] = [
		{
			accessorKey: 'name',
			header: ({ column }) =>
				renderComponent(DataTableSortHeader, {
					label: m.admin_routes_table_name(),
					sorted: column.getIsSorted(),
					onsort: () => column.toggleSorting(column.getIsSorted() === 'asc')
				})
		},
		{
			accessorKey: 'sector_id',
			header: m.admin_routes_table_sector(),
			enableSorting: false
		},
		{
			accessorKey: 'grade',
			header: ({ column }) =>
				renderComponent(DataTableSortHeader, {
					label: m.admin_routes_table_grade(),
					sorted: column.getIsSorted(),
					onsort: () => column.toggleSorting(column.getIsSorted() === 'asc')
				})
		},
		{
			accessorKey: 'first_ascent_climber_name',
			header: m.admin_routes_table_first_ascent_by(),
			enableSorting: false
		},
		{
			accessorKey: 'bolter_name',
			header: m.admin_routes_table_bolter(),
			enableSorting: false
		},
		{
			accessorKey: 'created_at',
			header: ({ column }) =>
				renderComponent(DataTableSortHeader, {
					label: m.admin_routes_table_created_at(),
					sorted: column.getIsSorted(),
					onsort: () => column.toggleSorting(column.getIsSorted() === 'asc')
				}),
			cell: ({ row }) => renderSnippet(createdAtSnippet, { value: row.original.created_at })
		},
		{
			accessorKey: 'updated_at',
			header: ({ column }) =>
				renderComponent(DataTableSortHeader, {
					label: m.admin_routes_table_updated_at(),
					sorted: column.getIsSorted(),
					onsort: () => column.toggleSorting(column.getIsSorted() === 'asc')
				}),
			cell: ({ row }) => renderSnippet(createdAtSnippet, { value: row.original.updated_at })
		},
		{
			id: 'actions',
			header: m.admin_routes_table_actions(),
			enableSorting: false,
			cell: ({ row }) => renderComponent(RouteTableActions, { routeId: row.original.id })
		}
	];
	// const routeTypeOptions = [
	// 	{ label: 'Sport', value: 'Sport' },
	// 	{ label: 'Traditional', value: 'Traditional' },
	// 	{ label: 'Boulder', value: 'Boulder' },
	// 	{ label: 'Mixed', value: 'Mixed' },
	// 	{ label: 'Aid', value: 'Aid' }
	// ];

	// const routeFilterConfig: ColumnFilterConfig<GetAllRoutesResponseItem> = {
	// 	name: {
	// 		type: 'string',
	// 		queryParameter: 'name',
	// 		placeholder: m.admin_routes_table_name()
	// 	},
	// 	sector_name: {
	// 		type: 'string',
	// 		queryParameter: 'sector_name',
	// 		placeholder: m.admin_routes_table_sector()
	// 	},
	// 	sector_id: {
	// 		type: 'string',
	// 		queryParameter: 'sector_id',
	// 		placeholder: m.admin_routes_table_sector()
	// 	},
	// 	area_name: {
	// 		type: 'string',
	// 		queryParameter: 'area_name',
	// 		placeholder: ''
	// 	},
	// 	area_id: {
	// 		type: 'string',
	// 		queryParameter: 'area_id',
	// 		placeholder: ''
	// 	},
	// 	type: {
	// 		type: 'select',
	// 		queryParameter: 'type',
	// 		placeholder: m.admin_routes_form_label_type(),
	// 		options: routeTypeOptions
	// 	},
	// 	created_at: {
	// 		type: 'date',
	// 		afterQueryParameter: 'created_after',
	// 		beforeQueryParameter: 'created_before',
	// 		label: m.admin_routes_table_created_at()
	// 	}
	// };

	// const { filterValues, setFilterValue, applyPagination } = useTableUrlFilters({
	// 	filterConfig: routeFilterConfig,
	// 	getServerFilters: () => data.filters
	// });

	// function onFilterValueChange(queryParameter: string, value: string, debounce: boolean) {
	// 	setFilterValue(queryParameter, value, debounce);
	// }

	const tableParams = useManualTableParams({
		getPagination: () => ({ page: data.pagination.page, pageSize: data.pagination.page_size }),
		getSorting: () => ({ sortBy: data.sort.sort_by, direction: data.sort.direction })
	});

	const table = createSvelteTable({
		get data() {
			return data.routes ?? [];
		},
		columns,
		getCoreRowModel: getCoreRowModel(),
		get rowCount() {
			return data.pagination.total_count;
		},
		state: {
			get pagination() {
				return tableParams.pagination;
			},
			get sorting() {
				return tableParams.sorting;
			}
		},
		onPaginationChange: tableParams.onPaginationChange,
		onSortingChange: tableParams.onSortingChange,
		manualFiltering: true,
		manualPagination: true,
		manualSorting: true
	});

	// const hasActiveFilters = $derived(
	// 	Boolean(
	// 		data.filters.name ||
	// 			data.filters.sector_name ||
	// 			data.filters.sector_id ||
	// 			data.filters.area_name ||
	// 			data.filters.area_id ||
	// 			data.filters.type ||
	// 			data.filters.created_before ||
	// 			data.filters.created_after
	// 	)
	// );
</script>

<div class="flex flex-col gap-6">
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
				<DataTableView
					{table}
					emptyMessage={m.admin_routes_empty_title()}
				>
					<!--
					{#snippet toolbar()}
						<DataTableFilters
							{columns}
							filterConfig={routeFilterConfig}
							values={filterValues}
							onValueChange={onFilterValueChange}
						/>
					{/snippet}
					-->
				</DataTableView>
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
