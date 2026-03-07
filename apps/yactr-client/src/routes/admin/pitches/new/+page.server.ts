import { createPitch, getAllSectors } from "$lib/api";
import { fail, redirect, } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";



export const load: PageServerLoad = async (event) => {
  const { session } = await event.parent();
  const { data: sectors } = await getAllSectors({
    headers: { Authorization: `Bearer ${session!.access_token}` }
  });
  return { sectors };
}

export const actions = {
  default: async ({ locals, request }) => {
    const session = await locals.auth();
    const data = await request.formData();

    if (!data.get("sector_id")) {
      throw fail(422, { sector_id: "sector_id" })
    }

    const { error } = await createPitch({
      headers: { Authorization: `Bearer ${session!.access_token}` },
      body: {
        sector_id: data.get("sector_id")!.toString(),
        name: data.get("name")?.toString() ?? undefined,
        type: data.get("type")?.toString() as any,
        description: data.get("description")?.toString() ?? undefined,
        grade: data.get("grade")?.toString() ?? undefined
      }
    });

    if (error) throw fail(422, { error });
    return redirect(303, "/admin/pitches");
  }
} satisfies Actions;


