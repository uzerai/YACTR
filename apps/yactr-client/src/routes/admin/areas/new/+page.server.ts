import { yactrEndpointsAreasCreateArea } from "$lib/api";
import { fail, redirect, type Actions } from "@sveltejs/kit";
import type { Coordinate } from "ol/coordinate.js";

export const actions = {
  default: async ({ request }) => {
    const data = await request.formData();

    if (data.get("name") === "") {
      return fail(422, { name: "name" })
    }

    if (data.get("description") === "") {
      return fail(422, { description: "description" })
    }

    const boundary: Coordinate[][][] = JSON.parse(data.get("boundary")!.toString())

    for (const multiPolygon of boundary) {
      for (const polygon of multiPolygon) {
        polygon.push(polygon.at(0)!)
      }
    }

    const { error, response } = await yactrEndpointsAreasCreateArea({
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

    if (error || !response.ok) {
      return fail(422, { error })
    }

    return redirect(303, '/admin/areas');
  }
} satisfies Actions;