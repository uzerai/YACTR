import { fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { yactrEndpointsAreasDeleteArea, yactrEndpointsAreasGetAllAreas } from "$lib/api";

export const load: PageServerLoad = async () => {
  const { data: areas } = await yactrEndpointsAreasGetAllAreas();

  return {
    areas
  }
}

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const area_id = data.get("area_id")!.toString();

    const { error } = await yactrEndpointsAreasDeleteArea({
      path: {
        area_id
      }
    });

    if (error) {
      return fail(422, { error })
    }
  }
}