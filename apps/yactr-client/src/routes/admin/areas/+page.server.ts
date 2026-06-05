import { error, fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { deleteArea, getAllAreas, type GetAllAreasData } from "$lib/api";
import { parsePaginationQuery } from "$lib/utils/pagination-query";

export const load: PageServerLoad = async ({ url }) => {
  // const name = url.searchParams.get("name")?.trim() || "";
  // const country_name = url.searchParams.get("country_name")?.trim() || "";
  // const created_before = url.searchParams.get("created_before")?.trim() || "";
  // const created_after = url.searchParams.get("created_after")?.trim() || "";
  const { page, page_size } = parsePaginationQuery(url.searchParams);

  const query: GetAllAreasData["query"] = {};

  // if (name) query.name = name;
  // if (country_name) query.country_name = country_name;
  // if (created_before) query.created_before = created_before;
  // if (created_after) query.created_after = created_after;

  query.page = page;
  query.page_size = page_size;

  const { data } = await getAllAreas({
    query,
  });

  if (data === undefined) {
    throw error(500, { message: "Failed to fetch areas" });
  }

  return {
    areas: data.items,
    pagination: {
      page,
      page_size,
      total_count: data.total_count,
    },
    // filters: {
    //   name,
    //   country_name,
    //   created_before,
    //   created_after,
    // },
  }
}

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const area_id = data.get("area_id")!.toString();

    const { error } = await deleteArea({
      path: {
        area_id
      }
    });

    if (error) {
      return fail(422, { error })
    }
  }
}
