import { error, fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { deleteArea, getAllAreas } from "$lib/api";

export const load: PageServerLoad = async () => {
  const { data } = await getAllAreas();

  if (data === undefined) {
    throw error(500, { message: "Failed to fetch areas" });
  }

  return {
    areas: data.items
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