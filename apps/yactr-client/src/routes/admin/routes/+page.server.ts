import { yactrEndpointsRoutesDeleteRoute, yactrEndpointsRoutesGetAllRoutes } from "$lib/api";
import { fail } from "@sveltejs/kit";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async () => {
  const { data: routes } = await yactrEndpointsRoutesGetAllRoutes();

  return {
    routes,
  }
}

export const actions = {
  delete: async ({ request }) => {
    const data = await request.formData();
    const route_id = data.get("route_id")!.toString();

    const { error, response } = await yactrEndpointsRoutesDeleteRoute({
      path: {
        route_id
      },
    });

    if (!response.ok) {
      return fail(422, { error })
    }
  }
}