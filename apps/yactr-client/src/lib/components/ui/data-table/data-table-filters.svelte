<script lang="ts" generics="TData extends RowData">
	import type { ColumnDef, RowData } from "@tanstack/table-core";
	import { Input } from "$lib/components/ui/input";
	import {
		getDataTableFilterFields,
		type ColumnFilterConfig
	} from "$lib/components/ui/data-table/filter-config";

	type Props = {
		columns: ColumnDef<TData>[];
		filterConfig: ColumnFilterConfig<TData>;
		values: Record<string, string>;
		onValueChange: (accessorKey: string, value: string) => void;
	};

	let { columns, filterConfig, values, onValueChange }: Props = $props();

	const fields = $derived(getDataTableFilterFields(columns, filterConfig));

	function placeholderFor(field: (typeof fields)[number]): string {
		return field.placeholder ?? (field.label || undefined) ?? field.accessorKey;
	}
</script>

<div class="flex flex-wrap items-end gap-3 pb-4">
	{#each fields as field (field.accessorKey)}
		{#if field.label}
			<div class="flex w-full flex-col gap-1 sm:w-56">
				<label for="filter-{field.accessorKey}" class="text-sm text-muted-foreground">{field.label}</label>
				<Input
					id="filter-{field.accessorKey}"
					type={field.inputType === "datetime-local" ? "datetime-local" : "text"}
					class={field.className ?? "w-full sm:w-56"}
					value={values[field.accessorKey] ?? ""}
					placeholder={field.placeholder}
					oninput={(e) => onValueChange(field.accessorKey, e.currentTarget.value)}
				/>
			</div>
		{:else}
			<Input
				class={field.className ?? "w-full sm:w-56"}
				type={field.inputType === "datetime-local" ? "datetime-local" : "text"}
				value={values[field.accessorKey] ?? ""}
				placeholder={placeholderFor(field)}
				oninput={(e) => onValueChange(field.accessorKey, e.currentTarget.value)}
			/>
		{/if}
	{/each}
</div>
