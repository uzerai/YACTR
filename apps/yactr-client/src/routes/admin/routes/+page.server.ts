import { deleteRoute, getAllRoutes } from "$lib/api";
import { error, fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async () => {
  const { data } = await getAllRoutes();

  if (data === undefined) {
    throw error(500, { message: "Failed to fetch routes" });
  }

  return {
    routes: data.items
  }
}

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const route_id = data.get("route_id")!.toString();

    const { error, response } = await deleteRoute({
      path: {
        route_id
      },
    });

    if (!response.ok) {
      return fail(422, { error })
    }
  }
}