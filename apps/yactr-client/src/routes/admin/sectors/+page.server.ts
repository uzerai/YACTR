import { error, fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { deleteSector, getAllSectors } from "$lib/api";
import { parsePaginationQuery } from "$lib/utils/pagination-query";

export const load: PageServerLoad = async ({ url }) => {
  const { page, page_size } = parsePaginationQuery(url.searchParams);

  const { data } = await getAllSectors({
    query: {
      page,
      page_size
    }
  });

  if (data === undefined) {
    throw error(500, { message: "Failed to fetch sectors" });
  }

  return {
    sectors: data.items,
    pagination: {
      page,
      page_size,
      total_count: data.total_count,
      page_count: Math.max(1, Math.ceil(data.total_count / page_size))
    }
  }
}

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const sector_id = data.get("sector_id")!.toString();

    const { error, response } = await deleteSector({
      path: {
        sector_id
      }
    });

    if (error) {
      return fail(422, { error })
    }
  }
}

