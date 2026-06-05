import { goto } from "$app/navigation";
import { page } from "$app/state";
import { getAllAreas, type GetAllAreasData, type GetAllAreasResponses, type Options } from "$lib/api";
import type { ApiFunctionType } from "$lib/api/api-function-type";
import type { RequestResult } from "$lib/api/generated/client";
import type { PaginationState, RowData, Updater } from "@tanstack/table-core";
import { SvelteURL } from "svelte/reactivity";


type UseManualTableParamsProps = {
	// filterConfig?: ColumnFilterConfig<TData>;
	// getServerFilters?: () => QueryFilterValues;
	// debounceMs?: number;
  pageIndex: number;
  pageSize: number;
};

async function navigateIfChanged(nextUrl: SvelteURL) {
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

export function useManualTableParams({
  // filterConfig = {},
  // getServerFilters = () => ({}),
  // debounceMs = DEFAULT_DEBOUNCE_MS,
  pageIndex,
  pageSize
}: UseManualTableParamsProps) {
	
	let pagination = $state<PaginationState>({ pageIndex, pageSize });

	const onPaginationChange = (updater: Updater<PaginationState>) => {
		const nextPagination = typeof updater === 'function' ? updater(pagination) : updater;
		pagination = nextPagination;

		const nextUrl = new SvelteURL(page.url);
		nextUrl.searchParams.set('page', String(nextPagination.pageIndex + 1));
		nextUrl.searchParams.set('page_size', String(nextPagination.pageSize));

		navigateIfChanged(nextUrl);
	};
 
	return { pagination, onPaginationChange };
}
