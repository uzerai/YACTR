import { deleteRoute, getAllRoutes } from "$lib/api";
// import type { ClimbingType } from "$lib/api";
import { zGetAllRoutesSortBy, zSortDirection } from "$lib/api/generated/zod.gen";
import { error, fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { parsePaginationQuery } from "$lib/utils/pagination-query";

export const load: PageServerLoad = async ({ url }) => {
  const { page, page_size } = parsePaginationQuery(url.searchParams);

  // Invalid or absent sort params fall back to the server's default sort.
  const sortByParse = zGetAllRoutesSortBy.safeParse(url.searchParams.get("sort_by"));
  const directionParse = zSortDirection.safeParse(url.searchParams.get("direction"));
  const sort_by = sortByParse.success ? sortByParse.data : undefined;
  const direction = directionParse.success ? directionParse.data : undefined;

  const query: NonNullable<Parameters<typeof getAllRoutes>[0]>["query"] = {};

  query.page = page;
  query.page_size = page_size;
  if (sort_by) query.sort_by = sort_by;
  if (direction) query.direction = direction;

  const { data } = await getAllRoutes({
    query,
  });

  if (data === undefined) {
    throw error(500, { message: "Failed to fetch routes" });
  }

  return {
    routes: data.items,
    sort: {
      sort_by,
      direction,
    },
    pagination: {
      page,
      page_size,
      total_count: data.total_count,
      page_count: Math.max(1, Math.ceil(data.total_count / page_size)),
    },
  };
};

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const route_id = data.get("route_id")!.toString();

    const { error, response } = await deleteRoute({
      path: {
        route_id,
      }
    });

    if (response && !response.ok) {
      return fail(422, { error });
    }
  },
};