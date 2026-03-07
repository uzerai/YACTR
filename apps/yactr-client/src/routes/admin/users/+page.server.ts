import { getCurrentUser } from "$lib/api";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async (event) => {
  const { session } = await event.parent();

  // No list-all users endpoint in API types; fetch current user as placeholder
  const { data: me } = await getCurrentUser({
    headers: {
      Authorization: `Bearer ${session!.access_token}`
    }
  });

  return {
    me
  }
}


