import { goto } from "$app/navigation";
import { page } from "$app/state";
import type { PaginationState, RowData } from "@tanstack/table-core";
import type { ColumnFilterConfig } from "$lib/components/ui/data-table/filter-config";
import { DEFAULT_PAGE_PARAM, DEFAULT_PAGE_SIZE_PARAM } from "$lib/utils/pagination-query";

type QueryFilterValues = Record<string, string | null | undefined>;

const DEFAULT_DEBOUNCE_MS = 350;

type UseTableUrlFiltersOptions<TData extends RowData> = {
	filterConfig: ColumnFilterConfig<TData>;
	getServerFilters: () => QueryFilterValues;
	debounceMs?: number;
	pageParam?: string;
	pageSizeParam?: string;
};

export function useTableUrlFilters<TData extends RowData>({
	filterConfig,
	getServerFilters,
	debounceMs = DEFAULT_DEBOUNCE_MS,
	pageParam = DEFAULT_PAGE_PARAM,
	pageSizeParam = DEFAULT_PAGE_SIZE_PARAM
}: UseTableUrlFiltersOptions<TData>) {
	const filterEntries = Object.entries(filterConfig).flatMap(([accessorKey, definition]) => {
		if (!definition) return [];
		return [{ accessorKey, queryParam: definition.queryParam }];
	});

	const initialFilterValues: Record<string, string> = {};
	const serverFilters = getServerFilters();
	for (const { accessorKey, queryParam } of filterEntries) {
		initialFilterValues[accessorKey] = normalizeFilterValue(serverFilters[queryParam]);
	}

	let filterValues = $state<Record<string, string>>(initialFilterValues);
	let debounceTimer: ReturnType<typeof setTimeout> | null = null;

	function setFilterValue(accessorKey: string, value: string) {
		if (!(accessorKey in filterValues)) return;
		filterValues[accessorKey] = value;
	}

	function applyPagination(nextPagination: PaginationState) {
		const url = new URL(page.url);
		url.searchParams.set(pageParam, String(nextPagination.pageIndex + 1));
		url.searchParams.set(pageSizeParam, String(nextPagination.pageSize));
		void navigateIfChanged(url);
	}

	$effect(() => {
		const nextServerFilters = getServerFilters();
		for (const { accessorKey, queryParam } of filterEntries) {
			filterValues[accessorKey] = normalizeFilterValue(nextServerFilters[queryParam]);
		}
	});

	$effect(() => {
		const nextServerFilters = getServerFilters();
		const hasChanged = filterEntries.some(({ accessorKey, queryParam }) => {
			return normalizeFilterValue(filterValues[accessorKey]) !== normalizeFilterValue(nextServerFilters[queryParam]);
		});

		if (!hasChanged) return;
		if (debounceTimer) clearTimeout(debounceTimer);

		const nextUrl = new URL(page.url);
		for (const { accessorKey, queryParam } of filterEntries) {
			const nextValue = normalizeFilterValue(filterValues[accessorKey]);
			if (nextValue) nextUrl.searchParams.set(queryParam, nextValue);
			else nextUrl.searchParams.delete(queryParam);
		}
		nextUrl.searchParams.delete(pageParam);

		debounceTimer = setTimeout(() => {
			void navigateIfChanged(nextUrl);
		}, debounceMs);

		return () => {
			if (debounceTimer) clearTimeout(debounceTimer);
		};
	});

	return {
		filterValues,
		setFilterValue,
		applyPagination
	};
}

function normalizeFilterValue(value: string | null | undefined): string {
	return value?.trim() ?? "";
}

async function navigateIfChanged(nextUrl: URL) {
	if (nextUrl.search === page.url.search) return;

	const search = nextUrl.searchParams.toString();
	const target = search ? `${nextUrl.pathname}?${search}` : nextUrl.pathname;

	await goto(target, {
		replaceState: true,
		noScroll: true,
		keepFocus: true,
		invalidateAll: true
	});
}
