import { createArea } from "$lib/api";
import { zCreateAreaRequest } from "$lib/api/generated/zod.gen";
import { fail, redirect } from "@sveltejs/kit";
import { superValidate } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import type { Actions, PageServerLoad } from "./$types";

export const load: PageServerLoad = async () => {
  const form = await superValidate(zod4(zCreateAreaRequest));
  return { form };
};

export const actions = {
  default: async ({ request }) => {
    const form = await superValidate(request, zod4(zCreateAreaRequest));

    if (!form.valid) {
      return fail(422, { form });
    }

    const { error, response  } = await createArea({
      body: {
        name: form.data.name,
        description: form.data.description ?? undefined,
        location: {
          type: "Point",
          coordinates: form.data.location?.coordinates
        },
        boundary: {
          type: "MultiPolygon",
          coordinates: form.data.boundary?.coordinates
        }
      }
    });

    if (!response.ok) {
      return fail(422, { form, error })
    }

    return redirect(303, '/admin/areas');
  }
} satisfies Actions;