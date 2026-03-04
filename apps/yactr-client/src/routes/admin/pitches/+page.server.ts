import { yactrEndpointsPitchesDeletePitch, yactrEndpointsPitchesGetAllPitches } from "$lib/api";
import { fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async (event) => {
  const { session } = await event.parent();

  const { data: pitches } = await yactrEndpointsPitchesGetAllPitches({
    headers: {
      Authorization: `Bearer ${session!.access_token}`
    }
  });

  return {
    pitches
  }
}

export const actions = {
  delete: async ({ locals, request }) => {
    const session = await locals.auth();

    const data = await request.formData();
    const pitch_id = data.get("pitch_id")!.toString();

    const { error } = await yactrEndpointsPitchesDeletePitch({
      path: {
        pitch_id
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


