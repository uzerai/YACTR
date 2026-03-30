import { deletePitch, getAllPitches } from "$lib/api";
import { error, fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async () => {
  const { data } = await getAllPitches();

  if (data === undefined) {
    throw error(500, { message: "Failed to fetch pitches" });
  }

  return {
    pitches: data.items
  }
}

export const actions = {
  delete: async ({ locals, request }) => {
    const data = await request.formData();
    const pitch_id = data.get("pitch_id")!.toString();

    const { error } = await deletePitch({
      path: {
        pitch_id
      }
    });

    if (error) {
      throw fail(422, { error })
    }
  }
}


