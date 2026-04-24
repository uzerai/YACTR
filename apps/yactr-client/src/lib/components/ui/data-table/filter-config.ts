import type { ColumnDef, RowData } from "@tanstack/table-core";

export type ColumnAccessorKey<TData extends RowData> = Extract<keyof TData, string> | (string & {});

type BaseColumnFilterDefinition = {
	label?: string;
	placeholder?: string;
	className?: string;
};

export type SelectFilterOption = {
	label: string;
	value: string;
};

export type StringColumnFilterDefinition = BaseColumnFilterDefinition & {
	type: "string";
	queryParameter: string;
};

type DateFilterWithBefore = {
	beforeQueryParameter: string;
	afterQueryParameter?: string;
};

type DateFilterWithAfter = {
	beforeQueryParameter?: string;
	afterQueryParameter: string;
};

export type DateColumnFilterDefinition = BaseColumnFilterDefinition & {
	type: "date";
} & (DateFilterWithBefore | DateFilterWithAfter);

export type SelectColumnFilterDefinition = BaseColumnFilterDefinition & {
	type: "select";
	queryParameter: string;
	options: SelectFilterOption[];
};

export type ColumnFilterDefinition =
	| StringColumnFilterDefinition
	| DateColumnFilterDefinition
	| SelectColumnFilterDefinition;

export type ColumnFilterConfig<TData extends RowData> = Partial<
	Record<ColumnAccessorKey<TData>, ColumnFilterDefinition>
>;

export type ResolvedColumnFilterDefinition = ColumnFilterDefinition & {
	accessorKey: string;
};

export type FilterQueryBinding = {
	accessorKey: string;
	queryParameter: string;
	part: "value" | "before" | "after";
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
): ResolvedColumnFilterDefinition[] {
	const resolved: ResolvedColumnFilterDefinition[] = [];

	for (const column of columns) {
		const accessorKey = asString((column as ColumnWithAccessorKey<TData>).accessorKey);
		if (!accessorKey) continue;

		const config = filterConfig[accessorKey as keyof typeof filterConfig];
		if (!config) continue;

		resolved.push({
			accessorKey,
			...config,
			label: config.label ?? getColumnLabel(column),
			placeholder: config.placeholder,
			className: config.className
		});
	}

	return resolved;
}

export function getOverarchingFilterFields<TData extends RowData>(
	columns: ColumnDef<TData>[],
	filterConfig: ColumnFilterConfig<TData>
): ResolvedColumnFilterDefinition[] {
	const columnFilterKeys = new Set(
		columns
			.map((column) => asString((column as ColumnWithAccessorKey<TData>).accessorKey))
			.filter(Boolean)
	);
	const extras: ResolvedColumnFilterDefinition[] = [];

	for (const accessorKey of Object.keys(filterConfig) as ColumnAccessorKey<TData>[]) {
		const def = filterConfig[accessorKey];
		if (!def || columnFilterKeys.has(accessorKey)) continue;
		extras.push({
			accessorKey,
			label: def.label,
			placeholder: def.placeholder,
			className: def.className,
			...def
		});
	}

	return extras;
}

/** Column-ordered filters first, then any remaining keys from `filterConfig` (insertion order). */
export function getDataTableFilterFields<TData extends RowData>(
	columns: ColumnDef<TData>[],
	filterConfig: ColumnFilterConfig<TData>
): ResolvedColumnFilterDefinition[] {
	return [...getResolvedColumnFilters(columns, filterConfig), ...getOverarchingFilterFields(columns, filterConfig)];
}

export function getFilterQueryBindings<TData extends RowData>(
	filterConfig: ColumnFilterConfig<TData>
): FilterQueryBinding[] {
	const bindings: FilterQueryBinding[] = [];

	for (const [accessorKey, definition] of Object.entries(filterConfig)) {
		if (!definition) continue;

		if (definition.type === "date") {
			if (definition.beforeQueryParameter) {
				bindings.push({
					accessorKey,
					queryParameter: definition.beforeQueryParameter,
					part: "before"
				});
			}
			if (definition.afterQueryParameter) {
				bindings.push({
					accessorKey,
					queryParameter: definition.afterQueryParameter,
					part: "after"
				});
			}
			continue;
		}

		bindings.push({
			accessorKey,
			queryParameter: definition.queryParameter,
			part: "value"
		});
	}

	return bindings;
}
