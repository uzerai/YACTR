import { yactrApiEndpointsAscentsDeleteAscent, yactrApiEndpointsAscentsGetAllAscents } from "$lib/api";
import { fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async (event) => {
  const { session } = await event.parent();

  const { data: ascents } = await yactrApiEndpointsAscentsGetAllAscents({
    headers: {
      Authorization: `Bearer ${session!.access_token}`
    }
  });

  return {
    ascents
  }
}

export const actions = {
  delete: async ({ locals, request }) => {
    const session = await locals.auth();

    const data = await request.formData();
    const ascent_id = data.get("ascent_id")!.toString();

    const { error } = await yactrApiEndpointsAscentsDeleteAscent({
      path: {
        ascent_id
      },
      headers: {
        Authorization: `Bearer ${session!.access_token}`
      }
    });

    if (error) {
      throw fail(422, { error })
    }
  }
}


