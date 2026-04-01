import type { ColumnDef, RowData } from "@tanstack/table-core";

export type FilterInputType = "text" | "datetime-local";

export type ColumnAccessorKey<TData extends RowData> = Extract<keyof TData, string> | (string & {});

export type ColumnFilterDefinition = {
	queryParam: string;
	inputType: FilterInputType;
	label?: string;
	placeholder?: string;
	className?: string;
};

export type ColumnFilterConfig<TData extends RowData> = Partial<
	Record<ColumnAccessorKey<TData>, ColumnFilterDefinition>
>;

export type ResolvedColumnFilter = ColumnFilterDefinition & {
	accessorKey: string;
};

type ColumnWithAccessorKey<TData extends RowData> = ColumnDef<TData> & {
	accessorKey?: unknown;
};

function asString(value: unknown): string {
	return typeof value === "string" ? value : "";
}

export function getColumnLabel<TData extends RowData>(column: ColumnDef<TData>): string {
	const header = (column as { header?: unknown }).header;
	return typeof header === "string" ? header : "";
}

export function getResolvedColumnFilters<TData extends RowData>(
	columns: ColumnDef<TData>[],
	filterConfig: ColumnFilterConfig<TData>
): ResolvedColumnFilter[] {
	const resolved: ResolvedColumnFilter[] = [];

	for (const column of columns) {
		const accessorKey = asString((column as ColumnWithAccessorKey<TData>).accessorKey);
		if (!accessorKey) continue;

		const config = filterConfig[accessorKey as keyof typeof filterConfig];
		if (!config) continue;

		resolved.push({
			accessorKey,
			queryParam: config.queryParam,
			inputType: config.inputType,
			label: config.label ?? getColumnLabel(column),
			placeholder: config.placeholder,
			className: config.className
		});
	}

	return resolved;
}

/** Column-ordered filters first, then any remaining keys from `filterConfig` (insertion order). */
export function getDataTableFilterFields<TData extends RowData>(
	columns: ColumnDef<TData>[],
	filterConfig: ColumnFilterConfig<TData>
): ResolvedColumnFilter[] {
	const fromColumns = getResolvedColumnFilters(columns, filterConfig);
	const seen = new Set(fromColumns.map((f) => f.accessorKey));
	const extras: ResolvedColumnFilter[] = [];

	for (const accessorKey of Object.keys(filterConfig) as ColumnAccessorKey<TData>[]) {
		const def = filterConfig[accessorKey];
		if (!def || seen.has(accessorKey)) continue;
		seen.add(accessorKey);
		extras.push({
			accessorKey,
			queryParam: def.queryParam,
			inputType: def.inputType,
			label: def.label,
			placeholder: def.placeholder,
			className: def.className
		});
	}

	return [...fromColumns, ...extras];
}
