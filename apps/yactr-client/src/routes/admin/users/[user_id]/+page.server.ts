// No user detail endpoint (other than /users/me) in API; placeholder only
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }) => {
  return { user_id: params.user_id };
}


