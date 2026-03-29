import { fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { deleteSector, getAllSectors } from "$lib/api";

export const load: PageServerLoad = async () => {
  const { data } = await getAllSectors();

  if (data === undefined) {
    return fail(500, "Failed to fetch sectors");
  }

  return {
    sectors: data.items
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

