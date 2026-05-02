import { deleteRoute, getAllRoutes, type ClimbingType } from "$lib/api";
import { error, fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { parsePaginationQuery } from "$lib/utils/pagination-query";

export const load: PageServerLoad = async ({ url }) => {
  const name = url.searchParams.get("name")?.trim() || "";
  const sector_name = url.searchParams.get("sector_name")?.trim() || "";
  const sector_id = url.searchParams.get("sector_id")?.trim() || "";
  const area_name = url.searchParams.get("area_name")?.trim() || "";
  const area_id = url.searchParams.get("area_id")?.trim() || "";
  const type = url.searchParams.get("type")?.trim() || "";
  const created_before = url.searchParams.get("created_before")?.trim() || "";
  const created_after = url.searchParams.get("created_after")?.trim() || "";
  const { page, page_size } = parsePaginationQuery(url.searchParams);

  const query: NonNullable<Parameters<typeof getAllRoutes>[0]>["query"] = {};

  if (name) query.name = name;
  if (sector_name) query.sector_name = sector_name;
  if (sector_id) query.sector_id = sector_id;
  if (area_name) query.area_name = area_name;
  if (area_id) query.area_id = area_id;
  if (type) query.type = type as ClimbingType;
  if (created_before) query.created_before = created_before;
  if (created_after) query.created_after = created_after;
  query.page = page;
  query.page_size = page_size;

  const { data } = await getAllRoutes({
    query,
  });

  if (data === undefined) {
    throw error(500, { message: "Failed to fetch routes" });
  }

  return {
    routes: data.items,
    pagination: {
      page,
      page_size,
      total_count: data.total_count,
      page_count: Math.max(1, Math.ceil(data.total_count / page_size)),
    },
    filters: {
      name,
      sector_name,
      sector_id,
      area_name,
      area_id,
      type,
      created_before,
      created_after,
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

    if (!response.ok) {
      return fail(422, { error });
    }
  },
};