import { fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { yactrApiEndpointsSectorsDeleteSector, yactrApiEndpointsSectorsGetAllSectors } from "$lib/api";

export const load: PageServerLoad = async () => {
  const { data: sectors } = await yactrApiEndpointsSectorsGetAllSectors();

  return {
    sectors
  }
}

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const sector_id = data.get("sector_id")!.toString();

    const { error, response } = await yactrApiEndpointsSectorsDeleteSector({
      path: {
        sector_id
      }
    });

    if (!response.ok) {
      return fail(422, { error })
    }
  }
}

