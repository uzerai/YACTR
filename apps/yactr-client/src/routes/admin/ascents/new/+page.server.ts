import { createAscent, type AscentType } from "$lib/api";
import { fail, redirect, type Actions } from "@sveltejs/kit";

export const actions = {
  default: async ({ locals, request }) => {
    const session = await locals.auth();

    const data = await request.formData();

    if (!data.get("route_id")) {
      throw fail(422, { route_id: "route_id" })
    }

    const { error } = await createAscent({
      headers: {
        Authorization: `Bearer ${session!.access_token}`
      },
      body: {
        route_id: data.get("route_id")!.toString(),
        type: Number(data.get("type")!.toString()) as AscentType,
        completed_at: data.get("completed_at")!.toString() || undefined,
      }
    });

    if (error) {
      throw fail(422, { error })
    }

    return redirect(303, "/admin/ascents");
  }
} satisfies Actions;


