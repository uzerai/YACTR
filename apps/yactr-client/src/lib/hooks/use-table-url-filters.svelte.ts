import { goto } from "$app/navigation";
import { page } from "$app/state";
import type { PaginationState, RowData } from "@tanstack/table-core";
import { getFilterQueryBindings, type ColumnFilterConfig } from "$lib/components/ui/data-table/filter-config";
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
	const filterEntries = getFilterQueryBindings(filterConfig);

	const initialFilterValues: Record<string, string> = {};
	const serverFilters = getServerFilters();
	for (const { queryParameter } of filterEntries) {
		initialFilterValues[queryParameter] = normalizeFilterValue(serverFilters[queryParameter]);
	}

	let filterValues = $state<Record<string, string>>(initialFilterValues);
	let debounceTimer: ReturnType<typeof setTimeout> | null = null;
	let shouldDebounce = $state(true);

	function setFilterValue(queryParameter: string, value: string, debounce = true) {
		if (!(queryParameter in filterValues)) return;
		filterValues[queryParameter] = value;
		shouldDebounce = debounce;
	}

	function clearFilterValue(queryParameter: string) {
		setFilterValue(queryParameter, "", false);
	}

	function applyPagination(nextPagination: PaginationState) {
		const url = new URL(page.url);
		url.searchParams.set(pageParam, String(nextPagination.pageIndex + 1));
		url.searchParams.set(pageSizeParam, String(nextPagination.pageSize));
		void navigateIfChanged(url);
	}

	$effect(() => {
		const nextServerFilters = getServerFilters();
		for (const { queryParameter } of filterEntries) {
			filterValues[queryParameter] = normalizeFilterValue(nextServerFilters[queryParameter]);
		}
	});

	$effect(() => {
		const nextServerFilters = getServerFilters();
		const hasChanged = filterEntries.some(({ queryParameter }) => {
			return (
				normalizeFilterValue(filterValues[queryParameter]) !==
				normalizeFilterValue(nextServerFilters[queryParameter])
			);
		});

		if (!hasChanged) return;
		if (debounceTimer) clearTimeout(debounceTimer);

		const nextUrl = new URL(page.url);
		for (const { queryParameter } of filterEntries) {
			const nextValue = normalizeFilterValue(filterValues[queryParameter]);
			if (nextValue) nextUrl.searchParams.set(queryParameter, nextValue);
			else nextUrl.searchParams.delete(queryParameter);
		}
		nextUrl.searchParams.delete(pageParam);

		if (shouldDebounce) {
			debounceTimer = setTimeout(() => {
				void navigateIfChanged(nextUrl);
			}, debounceMs);
		} else {
			void navigateIfChanged(nextUrl);
		}

		return () => {
			if (debounceTimer) clearTimeout(debounceTimer);
		};
	});

	return {
		filterValues,
		setFilterValue,
		clearFilterValue,
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
