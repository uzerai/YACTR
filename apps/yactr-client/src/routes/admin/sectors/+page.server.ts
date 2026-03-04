import { fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";
import { yactrEndpointsSectorsDeleteSector, yactrEndpointsSectorsGetAllSectors } from "$lib/api";

export const load: PageServerLoad = async () => {
  const { data: sectors } = await yactrEndpointsSectorsGetAllSectors();

  return {
    sectors
  }
}

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const sector_id = data.get("sector_id")!.toString();

    const { error, response } = await yactrEndpointsSectorsDeleteSector({
      path: {
        sector_id
      }
    });

    if (!response.ok) {
      return fail(422, { error })
    }
  }
}

