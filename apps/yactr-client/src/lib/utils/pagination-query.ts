type PaginationQueryOptions = {
	pageParamName?: string;
	pageSizeParamName?: string;
	defaultPage?: number;
	defaultPageSize?: number;
};

export type PaginationQueryResult = {
	page: number;
	page_size: number;
};

export const DEFAULT_PAGE_PARAM = "page";
export const DEFAULT_PAGE_SIZE_PARAM = "page_size";
export const DEFAULT_PAGE = 1;
export const DEFAULT_PAGE_SIZE = 25;

export function parsePaginationQuery(
	searchParams: URLSearchParams,
	options: PaginationQueryOptions = {}
): PaginationQueryResult {
	const pageParamName = options.pageParamName ?? DEFAULT_PAGE_PARAM;
	const pageSizeParamName = options.pageSizeParamName ?? DEFAULT_PAGE_SIZE_PARAM;
	const defaultPage = options.defaultPage ?? DEFAULT_PAGE;
	const defaultPageSize = options.defaultPageSize ?? DEFAULT_PAGE_SIZE;

	const page = parsePositiveNumber(searchParams.get(pageParamName), defaultPage);
	const page_size = parsePositiveNumber(searchParams.get(pageSizeParamName), defaultPageSize);

	return { page, page_size };
}

function parsePositiveNumber(value: string | null, fallback: number): number {
	const number = Number(value);

	if (!Number.isFinite(number) || number <= 0) return fallback;
	
	return Math.floor(number);
}
