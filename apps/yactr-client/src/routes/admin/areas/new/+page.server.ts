import { createArea } from "$lib/api";
import { zAreaRequestData } from "$lib/api/generated/zod.gen";
import { fail, redirect } from "@sveltejs/kit";
import { superValidate } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import type { Actions, PageServerLoad } from "./$types";

export const load: PageServerLoad = async () => {
  const form = await superValidate(zod4(zAreaRequestData));
  return { form };
};

export const actions = {
  default: async ({ request }) => {
    const form = await superValidate(request, zod4(zAreaRequestData));

    if (!form.valid) {
      console.log("form not valid");
      console.dir(form);
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

    if (error || !response.ok) {
      console.log("request failed");
      console.dir(response);
      return fail(422, { form, error })
    }

    return redirect(303, '/admin/areas');
  }
} satisfies Actions;