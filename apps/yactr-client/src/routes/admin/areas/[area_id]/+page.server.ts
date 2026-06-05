import { getAreaById, updateArea } from "$lib/api";
import { m } from "$lib/paraglide/messages.js";
import { error, fail, redirect } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import { superValidate } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import { zUpdateAreaBody } from "$lib/api/generated/zod.gen";

export const load: PageServerLoad = async ({ params }) => {
  const { data: area, response } = await getAreaById({
    path: {
      area_id: params.area_id
    }
  });

  if (!response.ok) {
    throw error(404, { message: m.admin_areas_error_not_found() });
  }

  const form = await superValidate(area, zod4(zUpdateAreaBody));

  return {
    form
  }
}

export const actions = {
  default: async ({ request, params }) => {
    const form = await superValidate(request, zod4(zUpdateAreaBody));

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

    return redirect(303, "/admin/areas");
  }
} satisfies Actions;