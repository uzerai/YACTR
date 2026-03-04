// No direct team retrieval route is present; show a placeholder using organizations list
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }) => {
  return { team_id: params.team_id };
}
