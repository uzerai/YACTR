import { uploadImage, createRoute, getAllSectors } from "$lib/api";
import { fail, redirect } from "@sveltejs/kit";
import type { PageServerLoad, Actions } from "./$types";
import { superValidate, withFiles } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import { routeManagementFormDto } from "$lib/components/forms";

export const load: PageServerLoad = async () => {
  const { data, response } = await getAllSectors();
  const form = await superValidate(zod4(routeManagementFormDto));

  if (!response.ok || data === undefined) {
    return fail(500, { message: "Failed to fetch sectors", form });
  }

  return { sectors: data.items, form };
}

export const actions: Actions = {
  default: async ({ request }) => {
    const form = await superValidate(request, zod4(routeManagementFormDto));

    if (!form.valid) {
      console.warn("Route form validation failed", { errors: form.errors, data: form.data });
      return fail(422, withFiles({ form }));
    }

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

    if (form.data.topo_image_overlay) {
      const { data: uploadData, error: uploadError, response: uploadResponse } = await uploadImage({
        body: { image: form.data.topo_image_overlay }
      });

      if (!uploadResponse.ok || !uploadData?.image_id) {
        return fail(424, withFiles({ form, error: uploadError ?? { message: "Failed to upload route topo overlay" } }));
      }

      topo_image_overlay_id = uploadData.image_id;
    }

    if (form.data.sector_topo_image_overlay) {
      const { data: uploadData, error: uploadError, response: uploadResponse } = await uploadImage({
        body: { image: form.data.sector_topo_image_overlay }
      });

      if (!uploadResponse.ok || !uploadData?.image_id) {
        return fail(424, withFiles({ form, error: uploadError ?? { message: "Failed to upload sector topo overlay" } }));
      }

      sector_topo_image_overlay_svg_id = uploadData.image_id;
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
      pitches: (form.data.pitches ?? []).map((pitch) => ({
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

    const { error, response } = await createRoute({ body });

    if (!response.ok) {
      return fail(422, withFiles({ form, error }));
    }

    return redirect(303, "/admin/routes");
  }
} satisfies Actions;
