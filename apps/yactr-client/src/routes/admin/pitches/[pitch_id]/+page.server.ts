import { getPitchById, updatePitch } from "$lib/api";
import { fail, type Actions } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";


export const load: PageServerLoad = async ({ params, parent }) => {
  const { data: pitch } = await getPitchById({
    path: { pitch_id: params.pitch_id! }
  });
  return { pitch };
}

export const actions = {
  default: async ({ locals, params, request }) => {
    const data = await request.formData();

    const { data: existingPitch, response: existingPitchResponse } = await getPitchById({
      path: { pitch_id: params.pitch_id! }
    });

    if (!existingPitchResponse.ok || !existingPitch) {
      throw fail(404, { error: { message: "Pitch not found" } });
    }

    const { error } = await updatePitch({
      path: { pitch_id: params.pitch_id! },
      body: {
        name: data.get("name")?.toString() ?? undefined,
        type: data.get("type")?.toString() as any,
        description: data.get("description")?.toString() ?? undefined,
        sector_id: existingPitch.sector_id,
        route_id: existingPitch.route_id,
        pitch_order: existingPitch.pitch_order ?? 0,
        grade: data.get("grade")?.toString() ?? undefined
      }
    });

    if (error) throw fail(422, { error });
  }
} satisfies Actions;


