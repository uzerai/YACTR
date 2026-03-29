import { uploadImage, getRouteById, updateRoute, getAllSectors, type TopoLinePoint } from "$lib/api";
import { routeManagementFormDto } from "$lib/components/forms";
import { error, fail, redirect } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import { superValidate, withFiles } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";

const areTopoPointsEqual = (left?: TopoLinePoint[] | null, right?: TopoLinePoint[] | null): boolean => {
  const leftPoints = left ?? [];
  const rightPoints = right ?? [];

  if (leftPoints.length !== rightPoints.length) return false;

  return leftPoints.every((point, index) => {
    const rightPoint = rightPoints[index];
    return rightPoint != null && point.x === rightPoint.x && point.y === rightPoint.y;
  });
};

export const load: PageServerLoad = async ({ params }) => {
  const { data: route } = await getRouteById({
    path: { route_id: params.route_id },
  });

  if (!route) throw error(404, { message: "Route not found" });

  const { data: sectorsData, response: sectorsResponse } = await getAllSectors();

  const form = await superValidate({
    ...route,
    pitches: (route.pitches ?? []).map((pitch, index) => ({
      id: pitch.id ?? undefined,
      name: pitch.name ?? "",
      type: pitch.type ?? route.type,
      pitch_order: pitch.pitch_order ?? index,
      gear_count: pitch.gear_count ?? undefined,
      description: pitch.description ?? undefined,
      grade: pitch.grade ?? undefined,
      height: pitch.height ?? undefined
    })),
    topo_line_points: route.topo_line_points ?? [],
    sector_topo_line_points: route.sector_topo_line_points ?? [],
    is_multipitch: (route.pitches?.length ?? 0) > 1
  }, zod4(routeManagementFormDto));

  if (!sectorsResponse.ok || !sectorsData) {
    return fail(500, { message: "Failed to fetch sectors", form });
  }

  return { route, sectors: sectorsData.items, form };
}

export const actions = {
  default: async ({ request, params }) => {
    const form = await superValidate(request, zod4(routeManagementFormDto));

    if (!form.valid) {
      return fail(422, withFiles({ form }));
    }

    const { data: existingRoute, response: existingRouteResponse } = await getRouteById({
      path: { route_id: params.route_id },
    });

    if (!existingRouteResponse.ok || !existingRoute) {
      return fail(404, withFiles({ form, error: { message: "Route not found" } }));
    }

    const routeTopoPointsChanged = !areTopoPointsEqual(existingRoute.topo_line_points, form.data.topo_line_points);
    const sectorTopoPointsChanged = !areTopoPointsEqual(existingRoute.sector_topo_line_points, form.data.sector_topo_line_points);

    let topo_image_id = form.data.topo_image_id ?? undefined;
    let topo_image_overlay_id = form.data.topo_image_overlay_id ?? undefined;
    let sector_topo_image_overlay_svg_id = form.data.sector_topo_image_overlay_svg_id ?? undefined;

    if (form.data.topo_image) {
      const { data: uploadData, error: uploadError, response: uploadResponse } = await uploadImage({
        body: { image: form.data.topo_image }
      });

      if (!uploadResponse.ok || !uploadData?.image_id) {
        return fail(424, withFiles({ form, error: uploadError ?? { message: "Failed to upload route topo image" } }));
      }

      topo_image_id = uploadData.image_id;
    }

    if (routeTopoPointsChanged && form.data.topo_image_overlay) {
      const { data: uploadData, error: uploadError, response: uploadResponse } = await uploadImage({
        body: { image: form.data.topo_image_overlay }
      });

      if (!uploadResponse.ok || !uploadData?.image_id) {
        return fail(424, withFiles({ form, error: uploadError ?? { message: "Failed to upload route topo overlay" } }));
      }

      topo_image_overlay_id = uploadData.image_id;
    }

    if (sectorTopoPointsChanged && form.data.sector_topo_image_overlay) {
      const { data: uploadData, error: uploadError, response: uploadResponse } = await uploadImage({
        body: { image: form.data.sector_topo_image_overlay }
      });

      if (!uploadResponse.ok || !uploadData?.image_id) {
        return fail(424, withFiles({ form, error: uploadError ?? { message: "Failed to upload sector topo overlay" } }));
      }

      sector_topo_image_overlay_svg_id = uploadData.image_id;
    }

    if (!form.data.is_multipitch) {
      if (form.data.pitches?.at(0)) {
        form.data.pitches = [form.data.pitches.at(0)!];
      } else {
        form.data.pitches = [];
      }
    }

    const body = {
      sector_id: form.data.sector_id,
      type: form.data.type,
      name: form.data.name,
      in_sector_order: form.data.in_sector_order ?? 0,
      description: form.data.description ?? undefined,
      height: form.data.height ?? undefined,
      grade: form.data.grade ?? undefined,
      gear_count: form.data.gear_count ?? undefined,
      first_ascent_climber_name: form.data.first_ascent_climber_name ?? undefined,
      bolter_name: form.data.bolter_name ?? undefined,
      pitches: form.data.pitches.map((pitch) => ({
        ...pitch,
        id: pitch.id ?? undefined,
        gear_count: pitch.gear_count ?? undefined,
        description: pitch.description ?? undefined,
        grade: pitch.grade ?? undefined,
        height: pitch.height ?? undefined
      })),
      topo_image_id,
      topo_image_overlay_id,
      sector_topo_image_id: form.data.sector_topo_image_id ?? undefined,
      sector_topo_image_overlay_svg_id,
      topo_line_points: form.data.topo_line_points ?? [],
      sector_topo_line_points: form.data.sector_topo_line_points ?? []
    };

    const { error: updateError, response: updateResponse } = await updateRoute({
      path: { route_id: params.route_id },
      body
    });

    if (!updateResponse.ok) {
      return fail(422, withFiles({ form, error: updateError }));
    }

    return redirect(303, "/admin/routes");
  }
} satisfies Actions;