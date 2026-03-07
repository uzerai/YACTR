import { uploadImage, getRouteById, updateRoute, getAllSectors, type TopoLinePoint } from "$lib/api";
import { error, fail, redirect } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ params }) => {
  const { data: route } = await getRouteById({
    path: { route_id: params.route_id },
  });

  if (!route) return error(404, "Not found");

  const { data: sectors } = await getAllSectors();

  console.dir(route, { depth: 4 });

  return { route, sectors };
}

export const actions = {
  default: async ({ request, params }) => {
    const data = await request.formData();

    if (!data.get("sector_id") || !data.get("name")) {
      return fail(422, { sector_id: "sector_id", name: "name" })
    }

    const route_image = data.get("route_image") as File;
    const route_image_overlay = data.get("route_image_svg_overlay") as File;
    const sector_image_overlay = data.get("sector_image_svg_overlay") as File;

    let topo_image_id: string | undefined = data.get("topo_image_id")?.toString();
    let topo_image_overlay_id: string | undefined;
    let sector_topo_image_overlay_svg_id: string | undefined;

    if (route_image.size !== 0) {
      const { data: route_image_data, response: route_image_upload_response } = await uploadImage({
        body: { image: route_image },
      });
      topo_image_id = route_image_data?.image_id;
      console.dir({ route_image_upload_response });
    }

    if (route_image_overlay.size !== 0) {
      const { data: route_image_overlay_data, response: route_overlay_response } = await uploadImage({
        body: { image: route_image_overlay },
      });
      topo_image_overlay_id = route_image_overlay_data?.image_id;
      console.dir({ route_overlay_response });
    }

    if (sector_image_overlay.size !== 0) {
      const { data: sector_image_overlay_data, response: sector_overlay_response } = await uploadImage({
        body: { image: sector_image_overlay }
      });
      sector_topo_image_overlay_svg_id = sector_image_overlay_data?.image_id;
      console.dir({ sector_overlay_response });
    }

    let topo_line_points: TopoLinePoint[] = [];
    console.log("topo_line_points", data.get("topo_line_points")?.toString());
    if (data.get("topo_line_points")?.toString()) {
      topo_line_points = JSON.parse(data.get("topo_line_points")!.toString())
    }

    let sector_topo_line_points: TopoLinePoint[] = [];
    if (data.get("sector_topo_line_points")?.toString()) {
      sector_topo_line_points = JSON.parse(data.get("sector_topo_line_points")!.toString());
    }

    const body = {
      sector_id: data.get("sector_id")!.toString(),
      name: data.get("name")!.toString(),
      grade: data.get("grade") != null ? Number(data.get("grade")) : undefined,
      pitches: [],
      description: data.get("description")?.toString(),
      first_ascent_climber_name: data.get("first_ascent_climber_name")?.toString(),
      bolter_name: data.get("bolter_name")?.toString(),
      topo_image_id,
      topo_image_overlay_id,
      sector_topo_image_id: data.get("sector_topo_image_id")?.toString(),
      sector_topo_image_overlay_svg_id,
      topo_line_points,
      sector_topo_line_points
    };

    console.dir(body, { depth: 4 });

    const { error, response } = await updateRoute({
      path: { route_id: params.route_id },
      body
    });

    if (!response.ok) {
      return fail(422, { error })
    }

    return redirect(303, "/admin/routes");
  }
} satisfies Actions;