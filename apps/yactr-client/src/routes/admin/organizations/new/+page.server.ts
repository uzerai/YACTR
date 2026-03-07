import { createOrganization } from "$lib/api";
import { fail, redirect, type Actions } from "@sveltejs/kit";

export const actions = {
  default: async ({ locals, request }) => {
    const session = await locals.auth();
    const data = await request.formData();

    if (!data.get("name")) throw fail(422, { name: "name" });

    const { error } = await createOrganization({
      headers: { Authorization: `Bearer ${session!.access_token}` },
      body: { name: data.get("name")!.toString() }
    });

    if (error) throw fail(422, { error });
    return redirect(303, "/admin/organizations");
  }
} satisfies Actions;


