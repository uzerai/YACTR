<script lang="ts" generics="TData extends RowData">
	import type { ColumnDef, RowData } from "@tanstack/table-core";
	import { Input } from "$lib/components/ui/input";
	import { Badge } from "$lib/components/ui/badge";
	import * as Popover from "$lib/components/ui/popover";
	import { HugeiconsIcon as Icon } from "@hugeicons/svelte";
	import { FilterIcon } from "@hugeicons/core-free-icons";
	import {
		getDataTableFilterFields,
		type ColumnFilterConfig
	} from "$lib/components/ui/data-table/filter-config";

	type Props = {
		columns: ColumnDef<TData>[];
		filterConfig: ColumnFilterConfig<TData>;
		values: Record<string, string>;
		onValueChange: (queryParameter: string, value: string, debounce: boolean) => void;
	};

	let { columns, filterConfig, values, onValueChange }: Props = $props();
	let isOpen = $state(false);
	let selectedFilterKey = $state<string | null>(null);

	const fields = $derived(getDataTableFilterFields(columns, filterConfig));
	const resolvedSelectedFilterKey = $derived.by(() => {
		if (selectedFilterKey && fields.some((field) => field.accessorKey === selectedFilterKey)) {
			return selectedFilterKey;
		}
		return fields[0]?.accessorKey ?? null;
	});
	const selectedFilter = $derived(
		resolvedSelectedFilterKey
			? fields.find((field) => field.accessorKey === resolvedSelectedFilterKey)
			: undefined
	);
	const activeFilterPills = $derived.by(() => {
		const pills: { key: string; label: string; queryParameter: string; value: string }[] = [];

		for (const field of fields) {
			if (field.type === "date") {
				if (field.afterQueryParameter) {
					const value = values[field.afterQueryParameter] ?? "";
					if (value) {
						pills.push({
							key: `${field.afterQueryParameter}-after`,
							label: `${field.label ?? field.accessorKey} after`,
							queryParameter: field.afterQueryParameter,
							value
						});
					}
				}
				if (field.beforeQueryParameter) {
					const value = values[field.beforeQueryParameter] ?? "";
					if (value) {
						pills.push({
							key: `${field.beforeQueryParameter}-before`,
							label: `${field.label ?? field.accessorKey} before`,
							queryParameter: field.beforeQueryParameter,
							value
						});
					}
				}
				continue;
			}

			const value = values[field.queryParameter] ?? "";
			if (!value) continue;
			if (field.type === "select") {
				const selectedOption = field.options.find((option) => option.value === value);
				pills.push({
					key: field.queryParameter,
					label: field.label || field.accessorKey,
					queryParameter: field.queryParameter,
					value: selectedOption?.label ?? value
				});
				continue;
			}

			pills.push({
				key: field.queryParameter,
				label: field.label || field.accessorKey,
				queryParameter: field.queryParameter,
				value
			});
		}

		return pills;
	});

	function placeholderFor(field: (typeof fields)[number]): string {
		return field.placeholder ?? (field.label || undefined) ?? field.accessorKey;
	}

	function selectFilter(accessorKey: string) {
		selectedFilterKey = accessorKey;
	}

	function clearFilter(queryParameter: string) {
		onValueChange(queryParameter, "", false);
	}
</script>

<div class="flex flex-wrap items-center gap-2 pb-4">
	<Popover.Root bind:open={isOpen}>
		<Popover.Trigger class="border-border bg-background hover:bg-muted h-7 rounded-[min(var(--radius-md),12px)] border px-2.5 text-[0.8rem] inline-flex items-center gap-1">
			<Icon icon={FilterIcon} class="size-3.5" />
			Filters
		</Popover.Trigger>
		<Popover.Content align="start" class="w-[min(92vw,44rem)] p-0">
			<div class="grid grid-cols-1 divide-y sm:grid-cols-[14rem_1fr] sm:divide-x sm:divide-y-0">
				<div class="max-h-80 overflow-auto p-2">
					<div class="mb-1 px-2 py-1 text-xs font-medium text-muted-foreground">Available filters</div>
					{#each fields as field (field.accessorKey)}
						<button
							type="button"
							class="hover:bg-muted w-full rounded-md px-2 py-1.5 text-left text-sm"
							class:bg-muted={resolvedSelectedFilterKey === field.accessorKey}
							onclick={() => selectFilter(field.accessorKey)}
						>
							{field.label || field.accessorKey}
						</button>
					{/each}
				</div>
				<div class="min-h-52 p-4">
					{#if selectedFilter}
						<div class="mb-3 text-sm font-medium">{selectedFilter.label || selectedFilter.accessorKey}</div>
						{#if selectedFilter.type === "date"}
							<div class="grid grid-cols-1 gap-2 sm:grid-cols-2">
								{#if selectedFilter.afterQueryParameter}
									<div class="flex flex-col gap-1">
										<label class="text-xs text-muted-foreground" for="filter-after-{selectedFilter.accessorKey}">
											After
										</label>
										<Input
											id="filter-after-{selectedFilter.accessorKey}"
											class={selectedFilter.className ?? "w-full"}
											type="datetime-local"
											value={values[selectedFilter.afterQueryParameter] ?? ""}
											oninput={(e) =>
												onValueChange(selectedFilter.afterQueryParameter!, e.currentTarget.value, false)}
										/>
									</div>
								{/if}
								{#if selectedFilter.beforeQueryParameter}
									<div class="flex flex-col gap-1">
										<label class="text-xs text-muted-foreground" for="filter-before-{selectedFilter.accessorKey}">
											Before
										</label>
										<Input
											id="filter-before-{selectedFilter.accessorKey}"
											class={selectedFilter.className ?? "w-full"}
											type="datetime-local"
											value={values[selectedFilter.beforeQueryParameter] ?? ""}
											oninput={(e) =>
												onValueChange(selectedFilter.beforeQueryParameter!, e.currentTarget.value, false)}
										/>
									</div>
								{/if}
							</div>
						{:else if selectedFilter.type === "select"}
							<select
								id="filter-{selectedFilter.accessorKey}"
								class={selectedFilter.className ??
									"border-input bg-background ring-offset-background placeholder:text-muted-foreground focus-visible:ring-ring flex h-8 w-full rounded-md border px-3 py-2 text-sm focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-50"}
								value={values[selectedFilter.queryParameter] ?? ""}
								onchange={(e) => onValueChange(selectedFilter.queryParameter, e.currentTarget.value, false)}
							>
								<option value="">{placeholderFor(selectedFilter)}</option>
								{#each selectedFilter.options as option (option.value)}
									<option value={option.value}>{option.label}</option>
								{/each}
							</select>
						{:else}
							<Input
								id="filter-{selectedFilter.accessorKey}"
								class={selectedFilter.className ?? "w-full"}
								value={values[selectedFilter.queryParameter] ?? ""}
								placeholder={placeholderFor(selectedFilter)}
								oninput={(e) => onValueChange(selectedFilter.queryParameter, e.currentTarget.value, true)}
							/>
						{/if}
					{:else}
						<p class="text-sm text-muted-foreground">No filters available.</p>
					{/if}
				</div>
			</div>
		</Popover.Content>
	</Popover.Root>

	{#if !isOpen}
		{#each activeFilterPills as pill (pill.key)}
			<Badge variant="secondary" class="gap-1 pr-1">
				<span>{pill.label}: {pill.value}</span>
				<button
					type="button"
					class="hover:bg-muted rounded-full p-0.5 text-muted-foreground hover:text-foreground"
					aria-label={`Remove ${pill.label} filter`}
					onclick={() => clearFilter(pill.queryParameter)}
				>
					X
				</button>
			</Badge>
		{/each}
	{/if}
</div>
