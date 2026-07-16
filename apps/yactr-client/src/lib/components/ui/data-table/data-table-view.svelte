<script lang="ts" generics="TData extends RowData">
	import type { RowData, Table as TanstackTable } from '@tanstack/table-core';
	import type { Snippet } from 'svelte';
	import FlexRender from './flex-render.svelte';
	import * as Table from '$lib/components/ui/table';
	import * as Pagination from '$lib/components/ui/pagination';
	import * as Select from '$lib/components/ui/select';

	type Props = {
		table: TanstackTable<TData>;
		columnCount?: number;
		emptyMessage?: string;
		toolbar?: Snippet;
		paginationSummary?: Snippet;
		showPaginationControls?: boolean;
		pageSizeOptions?: number[];
	};

	let {
		table,
		columnCount,
		emptyMessage = '',
		toolbar,
		paginationSummary,
		showPaginationControls = true,
		pageSizeOptions = [10, 25, 50, 100]
	}: Props = $props();

	const colSpan = $derived(columnCount ?? table.getVisibleLeafColumns().length);
	// because page index is 0 indexed, but pagination is 1 indexed, add 1
	const currentPage = $derived(table.getState().pagination.pageIndex + 1);
	const totalItems = $derived(table.getRowCount());
	const pageSize = $derived(table.getState().pagination.pageSize);
	const summaryFrom = $derived(totalItems === 0 ? 0 : (currentPage - 1) * pageSize + 1);
	const summaryTo = $derived(Math.min(currentPage * pageSize, totalItems));
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
{#if showPaginationControls}
	<div class="flex items-center justify-between py-4">
			{#if paginationSummary}
				{@render paginationSummary()}
			{:else}
				<span class="text-sm text-muted-foreground min-w-max">
					{summaryFrom} - {summaryTo} / <strong class="font-bold">{totalItems}</strong>
				</span>
			{/if}
			<div class="flex items-center gap-4">
				<Pagination.Root
					class="justify-end"
					count={totalItems}
					perPage={pageSize}
					page={currentPage}
					onPageChange={(pageNumber) => table.setPageIndex(pageNumber - 1)}
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
				<Select.Root
					type="single"
					value={String(pageSize)}
					onValueChange={(value) => table.setPagination({ pageIndex: 0, pageSize: Number(value) })}
				>
					<Select.Trigger class="w-[80px]">{pageSize}</Select.Trigger>
					<Select.Content>
						{#each pageSizeOptions as option (option)}
							<Select.Item value={String(option)} label={String(option)} />
						{/each}
					</Select.Content>
				</Select.Root>
			</div>
	</div>
{/if}
