import {
  getAllAreas,
  getSectorById,
  uploadImage,
  updateSector
} from "$lib/api";
import { m } from "$lib/paraglide/messages.js";
import { error, fail, redirect } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import { zod4 } from "sveltekit-superforms/adapters";
import { superValidate, withFiles } from "sveltekit-superforms";
import { sectorManagementFormDto } from "$lib/components/forms";

export const load: PageServerLoad = async ({ params }) => {

  const { data: sector, response } = await getSectorById({
    path: {
      sector_id: params.sector_id
    },
  });

  if (!response.ok || sector === undefined) {
    throw error(404, { message: m.admin_sectors_error_not_found() });
  }
  
  // Do mapping for front-end primary image indication here.
  sector.sector_images = sector.sector_images
    .map(image => ({ ...image, is_primary: image.image_id === sector.primary_sector_image_id }))
    .sort((a, b) => a.order - b.order);

  const form = await superValidate(sector, zod4(sectorManagementFormDto));

  const { data: areasData } = await getAllAreas();

  if (areasData === undefined) {
    throw error(500, { message: "Failed to fetch areas" });
  }

  return {
    form,
    sector,
    areas: areasData.items
  }
}

export const actions = {
  default: async ({ request, params }) => {
    const form = await superValidate(request, zod4(sectorManagementFormDto));

    if (!form.valid) {
      return fail(422, withFiles({ form }));
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

    const primary_sector_image_id = form.data.sector_images.find((img: { is_primary?: boolean; image_id?: string }) => img.is_primary)?.image_id;
    const sector_images = form.data.sector_images.map((img: { image_id?: string; order: number }) => ({
      image_id: img.image_id!,
      order: img.order
    }));

    const { error: updateError, response: updateResponse } = await updateSector({
      path: { sector_id: params.sector_id! },
      body: {
        name: form.data.name,
        area_id: form.data.area_id,
        primary_sector_image_id,
        sector_images,
        entry_point: form.data.entry_point ?? undefined,
        recommended_parking_location: form.data.recommended_parking_location ?? undefined,
        approach_path: form.data.approach_path ?? undefined,
        sector_area: form.data.sector_area
      }
    });

    if (!updateResponse.ok) {
      return fail(422, withFiles({ form, error: updateError }));
    }

    return redirect(303, "/admin/sectors");
  }
} satisfies Actions;

