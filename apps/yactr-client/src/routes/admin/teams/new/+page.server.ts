import { yactrApiEndpointsOrganizationsGetAllOrganizations, yactrApiEndpointsOrganizationsTeamsCreateOrganizationTeam } from "$lib/api";
import { fail, redirect } from "@sveltejs/kit";
import { type Actions, type PageServerLoad } from "./$types";

export const load: PageServerLoad = async (event) => {
  const { session } = await event.parent();
  const { data: organizations } = await yactrApiEndpointsOrganizationsGetAllOrganizations({
    headers: { Authorization: `Bearer ${session!.access_token}` }
  });
  return { organizations };
}

export const actions = {
  default: async ({ locals, request }) => {
    const session = await locals.auth();
    const data = await request.formData();

    if (!data.get("organization_id") || !data.get("name")) {
      throw fail(422, { organization_id: "organization_id", name: "name" })
    }

    const { error } = await yactrApiEndpointsOrganizationsTeamsCreateOrganizationTeam({
      path: { organization_id: data.get("organization_id")!.toString() },
      headers: { Authorization: `Bearer ${session!.access_token}` },
      body: { name: data.get("name")!.toString() }
    });

    if (error) throw fail(422, { error });
    return redirect(303, "/admin/teams");
  }
} satisfies Actions;


