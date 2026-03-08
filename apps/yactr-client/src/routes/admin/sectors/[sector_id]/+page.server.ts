import {
  getAllAreas,
  getSectorById,
  uploadImage,
  updateSector,
  type SectorImageRequestData
} from "$lib/api";
import { error, fail } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import { zod4 } from "sveltekit-superforms/adapters";
import { superValidate, withFiles } from "sveltekit-superforms";
import { sectorRequestWithImages } from "$lib/server/sector_request_with_images";

export const load: PageServerLoad = async ({ params }) => {

  const { data: sector } = await getSectorById({
    path: {
      sector_id: params.sector_id
    },
  });

  if (sector === undefined) {
    return error(404, {
      message: 'Not found'
    });
  }

  const form = await superValidate(sector, zod4(sectorRequestWithImages));

  sector.sector_images.map(image => ({ ...image, is_primary: image.image_id === sector.primary_sector_image_id }));

  const { data: areas } = await getAllAreas();

  return {
    form,
    sector,
    areas
  }
}

export const actions = {
  default: async ({ request, params }) => {
    const form = await superValidate(request, zod4(sectorRequestWithImages));

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

    const primary_sector_image_id = form.data.sector_images.find(img => img.is_primary)?.image_id;
    const sector_images = form.data.sector_images.map(img => ({
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
        entry_point: form.data.entry_point,
        recommended_parking_location: form.data.recommended_parking_location ?? undefined,
        approach_path: form.data.approach_path ?? undefined,
        sector_area: form.data.sector_area
      }
    });

    if (!updateResponse.ok) {
      return fail(422, withFiles({ form, error: updateError }));
    }

    return { form };
  }
} satisfies Actions;

