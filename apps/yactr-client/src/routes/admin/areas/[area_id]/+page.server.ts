import { yactrApiEndpointsAreasGetAreaById, yactrApiEndpointsAreasUpdateArea } from "$lib/api";
import { fail } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import type { Coordinate } from "ol/coordinate";
import { superValidate } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import { zYactrApiEndpointsAreasAreaRequestData } from "$lib/api/generated/zod.gen";

export const load: PageServerLoad = async ({ params }) => {
  const { data: area } = await yactrApiEndpointsAreasGetAreaById({
    path: {
      area_id: params.area_id
    }
  });

  const form = await superValidate(area, zod4(zYactrApiEndpointsAreasAreaRequestData));

  return {
    form
  }
}

export const actions = {
  default: async ({ request, params }) => {
    const data = await request.formData();

    const boundary = JSON.parse(data.get("boundary")!.toString()) as Coordinate[][][];

    // Complete the polygon
    for (const multiPolygon of boundary) {
      for (const polygon of multiPolygon) {
        polygon.push(polygon.at(0)!)
      }
    }

    const { error } = await yactrApiEndpointsAreasUpdateArea({
      path: {
        area_id: params.area_id!
      },
      body: {
        name: data.get("name")!.toString(),
        description: data.get("description")!.toString(),
        location: {
          type: "Point",
          coordinates: JSON.parse(data.get("location")!.toString())
        },
        boundary: {
          type: "MultiPolygon",
          coordinates: boundary
        }
      }
    });

    if (error) {
      console.error(error);
      return fail(422, { error });
    }
  }
} satisfies Actions;