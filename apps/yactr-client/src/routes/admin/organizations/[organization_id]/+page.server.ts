import { yactrApiEndpointsOrganizationsGetOrganizationById } from "$lib/api";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params, parent }) => {
  const { session } = await parent();
  const { data: organization } = await yactrApiEndpointsOrganizationsGetOrganizationById({
    path: { organization_id: params.organization_id! },
    headers: { Authorization: `Bearer ${session!.access_token}` }
  });
  return { organization };
}


