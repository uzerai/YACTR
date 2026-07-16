import { goto } from "$app/navigation";
import { page } from "$app/state";
import type { PaginationState, SortingState, Updater } from "@tanstack/table-core";

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

	const onPaginationChange = (updater: Updater<PaginationState>) => {
		const nextPagination = typeof updater === "function" ? updater(pagination) : updater;

		const nextUrl = new URL(page.url);
		nextUrl.searchParams.set("page", String(nextPagination.pageIndex + 1));
		nextUrl.searchParams.set("page_size", String(nextPagination.pageSize));

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
