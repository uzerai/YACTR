import { createPitch, getAllSectors } from "$lib/api";
import { error, fail, redirect, } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";

export const load: PageServerLoad = async (event) => {
  const { data } = await getAllSectors();

  if (data === undefined) {
    throw error(500, { message: "Failed to fetch sectors" });
  }

  return { sectors: data.items };
}

export const actions = {
  default: async ({ locals, request }) => {
    const data = await request.formData();

    if (!data.get("sector_id")) {
      throw fail(422, { sector_id: "sector_id" })
    }

    const { error } = await createPitch({
      body: {
        sector_id: data.get("sector_id")!.toString(),
        name: data.get("name")?.toString() ?? undefined,
        type: data.get("type")?.toString() as any,
        description: data.get("description")?.toString() ?? undefined,
        grade: data.get("grade")?.toString() ?? undefined,
        route_id: "",
        pitch_order: 0
      }
    });

    if (error) throw fail(422, { error });
    return redirect(303, "/admin/pitches");
  }
} satisfies Actions;


