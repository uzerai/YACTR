import { yactrApiEndpointsAreasCreateArea } from "$lib/api";
import { zYactrApiEndpointsAreasAreaRequestData } from "$lib/api/generated/zod.gen";
import { fail, redirect, type Actions } from "@sveltejs/kit";
import { superValidate } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";

export const load = async () => {
  const form = await superValidate(zod4(zYactrApiEndpointsAreasAreaRequestData));
  return { form };
};

export const actions = {
  default: async ({ request }) => {
    const form = await superValidate(request, zod4(zYactrApiEndpointsAreasAreaRequestData));

    if (!form.valid) {
      return fail(422, { form });
    }

    const { error, response  } = await yactrApiEndpointsAreasCreateArea({
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

    if (error || !response.ok) {
      return fail(422, { form, error })
    }

    return redirect(303, '/admin/areas');
  }
} satisfies Actions;