import { fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { yactrApiEndpointsAreasDeleteArea, yactrApiEndpointsAreasGetAllAreas } from "$lib/api";

export const load: PageServerLoad = async () => {
  const { data: areas } = await yactrApiEndpointsAreasGetAllAreas();

  return {
    areas
  }
}

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const area_id = data.get("area_id")!.toString();

    const { error } = await yactrApiEndpointsAreasDeleteArea({
      path: {
        area_id
      }
    });

    if (error) {
      return fail(422, { error })
    }
  }
}