import { yactrEndpointsImagesUploadImage, yactrEndpointsRoutesCreateRoute, yactrEndpointsSectorsGetAllSectors } from "$lib/api";
import { fail, redirect } from "@sveltejs/kit";
import type { PageServerLoad, Actions } from "./$types";

export const load: PageServerLoad = async () => {
  const { data: sectors } = await yactrEndpointsSectorsGetAllSectors();

  return { sectors };
}

export const actions = {
  default: async ({ request }) => {
    const formData = await request.formData();
    console.dir(Object.fromEntries(formData));

    if (!formData.get("sector_id") || !formData.get("name")) {
      return fail(422, { sector_id: "sector_id", name: "name" })
    }

    const route_image = formData.get("route_image") as File;
    const route_image_overlay = formData.get("route_image_svg_overlay") as File;
    const sector_image_overlay = formData.get("sector_image_svg_overlay") as File;

    const route_image_upload = yactrEndpointsImagesUploadImage({
      body: { image: route_image },
    });

    const route_image_overlay_upload = yactrEndpointsImagesUploadImage({
      body: { image: route_image_overlay },
    });

    const sector_image_overlay_upload = yactrEndpointsImagesUploadImage({
      body: { image: sector_image_overlay }
    });

    const { data: route_image_data } = await route_image_upload;
    const { data: route_image_overlay_data } = await route_image_overlay_upload;
    const { data: sector_image_overlay_data } = await sector_image_overlay_upload;

    // console.dir({
    //   route_image_response,
    //   route_overlay_response,
    //   sector_overlay_response
    // }, { depth: 4 });

    const { error, response } = await yactrEndpointsRoutesCreateRoute({
      body: {
        sector_id: formData.get("sector_id")!.toString(),
        name: formData.get("name")!.toString(),
        grade: formData.get("grade")?.toString(),
        pitches: [],
        description: formData.get("description")?.toString(),
        first_ascent_climber_name: formData.get("first_ascent_climber_name")?.toString(),
        bolter_name: formData.get("bolter_name")?.toString(),
        topo_image_id: route_image_data?.image_id,
        topo_image_overlay_id: route_image_overlay_data?.image_id,
        sector_topo_image_id: formData.get("sector_topo_image_id")?.toString(),
        sector_topo_image_overlay_svg_id: sector_image_overlay_data?.image_id,
        topo_line_points: JSON.parse(formData.get("topo_line_points")?.toString() ?? "[]")
      }
    });

    console.dir({
      response
    }, { depth: 4 });

    if (!response.ok) {
      return fail(422, { error })
    }

    return redirect(303, "/admin/routes");
  }
} satisfies Actions;
