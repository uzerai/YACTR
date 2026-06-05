<script lang="ts">
	import { getCoreRowModel, type ColumnDef, type PaginationState, type Updater } from '@tanstack/table-core';
	import { createRawSnippet } from 'svelte';
	import {
		createSvelteTable,
		DataTableView,
		renderComponent,
		renderSnippet
	} from '$lib/components/ui/data-table';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	import { Button } from '$lib/components/ui/button';
	import * as Empty from '$lib/components/ui/empty';
	import { m } from '$lib/paraglide/messages.js';
	import AreaTableActions from './area-table-actions.svelte';
	import type { GetAllAreasResponseItem } from '$lib/api';
	import { useManualTableParams } from '$lib/components/ui/data-table/use-manual-table-params.svelte';

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

	// const areaFilterConfig: ColumnFilterConfig<GetAllAreasResponseItem> = {
	// 	name: {
	// 		type: 'string',
	// 		queryParameter: 'name',
	// 		placeholder: m.admin_areas_table_name()
	// 	},
	// 	country_name: {
	// 		type: 'string',
	// 		queryParameter: 'country_name',
	// 		placeholder: ''
	// 	},
	// 	created_at: {
	// 		type: 'date',
	// 		afterQueryParameter: 'created_after',
	// 		beforeQueryParameter: 'created_before',
	// 		label: m.admin_areas_table_created_at()
	// 	}
	// };

	const { pagination, onPaginationChange } = useManualTableParams({
		pageIndex: data.pagination.page,
		pageSize: data.pagination.page_size
	});

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
				return pagination
			}
		},
		onPaginationChange,
		manualFiltering: true,
		manualPagination: true
	});
</script>

<div class="flex flex-col gap-6">
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
				<DataTableView
					{table}
					emptyMessage={m.admin_areas_empty_title()}
				>
					<!--
					{#snippet toolbar()}
						<DataTableFilters
							{columns}
							filterConfig={areaFilterConfig}
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
			<Empty.Title>{m.admin_areas_empty_title()}</Empty.Title>
			<Empty.Description>{m.admin_areas_empty_description()}</Empty.Description>
			<Empty.Content>
				<Button variant="default" href="/admin/areas/new">{m.admin_areas_create_area()}</Button>
			</Empty.Content>
		</Empty.Root>
	{/if}
</div>
