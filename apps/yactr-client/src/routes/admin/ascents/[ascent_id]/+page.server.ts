import { getAscentById, updateAscent, type AscentType } from "$lib/api";
import { fail } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params, parent }) => {
  const { session } = await parent();

  const { data: ascent } = await getAscentById({
    path: {
      ascent_id: params.ascent_id!
    },
    headers: {
      Authorization: `Bearer ${session!.access_token}`
    }
  });

  return { ascent };
}

export const actions = {
  default: async ({ locals, request, params }) => {
    const session = await locals.auth();
    const data = await request.formData();

    const typeValue = data.get("type");
    const body: { type?: AscentType; completed_at?: string } = {
      type: typeValue !== null && typeValue !== "" ? (Number(typeValue.toString()) as AscentType) : undefined,
      completed_at: data.get("completed_at")?.toString()
    };

    const { error } = await updateAscent({
      path: {
        ascent_id: params.ascent_id!
      },
      headers: {
        Authorization: `Bearer ${session!.access_token}`
      },
      body
    });

    if (error) {
      throw fail(422, { error })
    }
  }
} satisfies Actions;


