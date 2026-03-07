import { getAllOrganizations } from "$lib/api";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async (event) => {
  const { session } = await event.parent();

  const { data: organizations } = await getAllOrganizations({
    headers: {
      Authorization: `Bearer ${session!.access_token}`
    }
  });

  return {
    organizations
  }
}


