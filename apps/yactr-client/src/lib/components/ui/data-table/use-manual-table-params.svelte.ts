import { goto } from "$app/navigation";
import { page } from "$app/state";
import type { PaginationState, SortingState, Updater } from "@tanstack/table-core";
import { LocalStorage } from "$lib/utils/local-storage.svelte";
import { DEFAULT_PAGE_SIZE, DEFAULT_PAGE_SIZE_PARAM } from "$lib/utils/pagination-query";

/** Page-size preference shared by all data tables. */
const storedPageSize = new LocalStorage<number>("admin-data-table:page-size", DEFAULT_PAGE_SIZE);

function getStoredPageSize(): number | undefined {
	const value = Number(storedPageSize.current);
	return Number.isFinite(value) && value > 0 ? Math.floor(value) : undefined;
}

type ServerPagination = {
	/** 1-based page number, as returned by the load function. */
	page: number;
	pageSize: number;
};

type ServerSorting = {
	/** Column id / sort_by wire value (e.g. "created_at"). */
	sortBy?: string;
	direction?: string;
};

type UseManualTableParamsProps = {
	getPagination: () => ServerPagination;
	getSorting?: () => ServerSorting;
};

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

/**
 * Wires TanStack Table manual pagination/sorting to the page URL. State is derived
 * from the load function's data (via the getters), and changes navigate with
 * `invalidateAll` so the load function re-fetches.
 */
export function useManualTableParams({ getPagination, getSorting }: UseManualTableParamsProps) {
	const pagination = $derived.by<PaginationState>(() => {
		const { page: currentPage, pageSize } = getPagination();
		return { pageIndex: Math.max(0, currentPage - 1), pageSize };
	});

	const sorting = $derived.by<SortingState>(() => {
		const { sortBy, direction } = getSorting?.() ?? {};
		return sortBy ? [{ id: sortBy, desc: direction === "desc" }] : [];
	});

	// When the URL doesn't specify a page size, fall back to the stored preference.
	$effect(() => {
		if (page.url.searchParams.has(DEFAULT_PAGE_SIZE_PARAM)) return;

		const stored = getStoredPageSize();
		if (stored === undefined || stored === getPagination().pageSize) return;

		const nextUrl = new URL(page.url);
		nextUrl.searchParams.set(DEFAULT_PAGE_SIZE_PARAM, String(stored));

		void navigateIfChanged(nextUrl);
	});

	const onPaginationChange = (updater: Updater<PaginationState>) => {
		const nextPagination = typeof updater === "function" ? updater(pagination) : updater;

		// Remember explicitly selected page sizes across tables and sessions.
		if (nextPagination.pageSize !== pagination.pageSize) {
			storedPageSize.current = nextPagination.pageSize;
		}

		const nextUrl = new URL(page.url);
		nextUrl.searchParams.set("page", String(nextPagination.pageIndex + 1));
		nextUrl.searchParams.set(DEFAULT_PAGE_SIZE_PARAM, String(nextPagination.pageSize));

		void navigateIfChanged(nextUrl);
	};

	const onSortingChange = (updater: Updater<SortingState>) => {
		const nextSorting = typeof updater === "function" ? updater(sorting) : updater;
		const [sort] = nextSorting;

		const nextUrl = new URL(page.url);
		if (sort) {
			nextUrl.searchParams.set("sort_by", sort.id);
			nextUrl.searchParams.set("direction", sort.desc ? "desc" : "asc");
		} else {
			nextUrl.searchParams.delete("sort_by");
			nextUrl.searchParams.delete("direction");
		}
		// Changing the sort order invalidates the current page position.
		nextUrl.searchParams.delete("page");

		void navigateIfChanged(nextUrl);
	};

	return {
		get pagination() {
			return pagination;
		},
		get sorting() {
			return sorting;
		},
		onPaginationChange,
		onSortingChange
	};
}
