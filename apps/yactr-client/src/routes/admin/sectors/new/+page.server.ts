import { getAllAreas, uploadImage, createSector } from "$lib/api";
import { error, fail, redirect } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import { superValidate, withFiles } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import { sectorManagementFormDto } from "$lib/components/forms";

export const load: PageServerLoad = async () => {
  const { data  } = await getAllAreas();

  if (data === undefined) {
    throw error(500, "Failed to fetch areas");
  }

  const form = await superValidate(zod4(sectorManagementFormDto));

  return { areas: data.items, form };
}

export const actions = {
  default: async ({ request }) => {
    const form = await superValidate(request, zod4(sectorManagementFormDto));

    if (!form.valid) {
      return fail(422, withFiles({ form }));
    }

    if (form.data.sector_images.length === 0) {
      return fail(422, withFiles({ form, error: { message: "At least one sector image is required" } }));
    }

    for (const item of form.data.sector_images) {
      if (item.image_id) continue;
      if (!item.image) continue;

      const { data: uploadData, error: uploadError, response: uploadResponse } = await uploadImage({
        body: { image: item.image }
      });
      
      if (!uploadResponse.ok || !uploadData?.image_id) {
        return fail(424, withFiles({ form, error: uploadError ?? { message: "Failed to upload sector image" } }));
      }

      item.image_id = uploadData.image_id;
    }

    const primary_sector_image_id = form.data.sector_images.find((img: { is_primary?: boolean; image_id?: string }) => img.is_primary)?.image_id ?? form.data.sector_images[0]!.image_id;
    const sector_images = form.data.sector_images.map((img: { image_id?: string; order: number }) => ({
      image_id: img.image_id!,
      order: img.order
    }));

    const { error: create_sector_error, response: create_sector_response } = await createSector({
      body: {
        name: form.data.name,
        area_id: form.data.area_id,
        primary_sector_image_id,
        sector_images,
        entry_point: form.data.entry_point ?? undefined,
        recommended_parking_location: form.data.recommended_parking_location ?? undefined,
        approach_path: form.data.approach_path ?? undefined,
        sector_area: form.data.sector_area,
      }
    });

    if (!create_sector_response.ok) {
      return fail(422, withFiles({ form, error: create_sector_error }));
    }

    return redirect(303, "/admin/sectors");
  }
} satisfies Actions;

