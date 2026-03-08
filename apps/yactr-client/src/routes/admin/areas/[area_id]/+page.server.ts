import { getAreaById, updateArea } from "$lib/api";
import { error, fail } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import { superValidate } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import { zAreaRequestData } from "$lib/api/generated/zod.gen";

export const load: PageServerLoad = async ({ params }) => {
  const { data: area, response } = await getAreaById({
    path: {
      area_id: params.area_id
    }
  });

  if (!response.ok) {
    return error(404, { message: 'Not found' });
  }

  const form = await superValidate(area, zod4(zAreaRequestData));

  return {
    form
  }
}

export const actions = {
  default: async ({ request, params }) => {
    const form = await superValidate(request, zod4(zAreaRequestData));

    if (!form.valid) {
      return fail(422, { form });
    }

    const { error, response } = await updateArea({
      path: {
        area_id: params.area_id!
      },
      body: {
        name: form.data.name,
        description: form.data.description ?? undefined,
        location: form.data.location,
        boundary: form.data.boundary
      }
    });

    if (!response.ok) {
      console.dir({ response, error }, { depth: 4 });
      return fail(422, { form, error });
    }

    return { form };
  }
} satisfies Actions;