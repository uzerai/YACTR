<script lang="ts" generics="TData extends RowData">
	import type { RowData, Table as TanstackTable } from '@tanstack/table-core';
	import type { Snippet } from 'svelte';
	import FlexRender from './flex-render.svelte';
	import * as Table from '$lib/components/ui/table';
	import * as Pagination from '$lib/components/ui/pagination';

	type Props = {
		table: TanstackTable<TData>;
		columnCount?: number;
		emptyMessage?: string;
		toolbar?: Snippet;
		paginationSummary?: Snippet;
		showPaginationControls?: boolean;
	};

	let {
		table,
		columnCount,
		emptyMessage = '',
		toolbar,
		paginationSummary,
		showPaginationControls = true
	}: Props = $props();

	const colSpan = $derived(columnCount ?? table.getVisibleLeafColumns().length);
	const showFooter = $derived(Boolean(paginationSummary) || showPaginationControls);
	const currentPage = $derived(table.getState().pagination.pageIndex + 1);
	const totalItems = $derived(table.getRowCount());
	const pageSize = $derived(table.getState().pagination.pageSize);
</script>

{#if toolbar}
	{@render toolbar()}
{/if}
<div class="rounded-md border">
	<Table.Root>
		<Table.Header>
			{#each table.getHeaderGroups() as headerGroup (headerGroup.id)}
				<Table.Row>
					{#each headerGroup.headers as header (header.id)}
						<Table.Head colspan={header.colSpan}>
							{#if !header.isPlaceholder}
								<FlexRender
									content={header.column.columnDef.header}
									context={header.getContext()}
								/>
							{/if}
						</Table.Head>
					{/each}
				</Table.Row>
			{/each}
		</Table.Header>
		<Table.Body>
			{#each table.getRowModel().rows as row (row.id)}
				<Table.Row>
					{#each row.getVisibleCells() as cell (cell.id)}
						<Table.Cell>
							<FlexRender content={cell.column.columnDef.cell} context={cell.getContext()} />
						</Table.Cell>
					{/each}
				</Table.Row>
			{:else}
				<Table.Row>
					<Table.Cell colspan={colSpan} class="h-24 text-center">
						{emptyMessage}
					</Table.Cell>
				</Table.Row>
			{/each}
		</Table.Body>
	</Table.Root>
</div>
{#if showFooter}
	<div class="flex items-center justify-between py-4">
		{#if paginationSummary}
			{@render paginationSummary()}
		{:else}
			<span></span>
		{/if}
		{#if showPaginationControls}
			<Pagination.Root
				count={totalItems}
				perPage={pageSize}
				page={currentPage}
				onPageChange={(p) => table.setPageIndex(p - 1)}
			>
				{#snippet children({ pages, currentPage: activePage })}
					<Pagination.Content>
						<Pagination.Item>
							<Pagination.PrevButton />
						</Pagination.Item>
						{#each pages as p (p.key)}
							<Pagination.Item>
								{#if p.type === 'ellipsis'}
									<Pagination.Ellipsis />
								{:else}
									<Pagination.Link page={p} isActive={activePage === p.value} />
								{/if}
							</Pagination.Item>
						{/each}
						<Pagination.Item>
							<Pagination.NextButton />
						</Pagination.Item>
					</Pagination.Content>
				{/snippet}
			</Pagination.Root>
		{/if}
	</div>
{/if}
