import { yactrApiEndpointsAreasGetAreaById, yactrApiEndpointsAreasUpdateArea } from "$lib/api";
import { fail } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import type { Coordinate } from "ol/coordinate";

export const load: PageServerLoad = async ({ params }) => {
  const { data: area } = await yactrApiEndpointsAreasGetAreaById({
    path: {
      area_id: params.area_id
    }
  });

  return {
    area
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