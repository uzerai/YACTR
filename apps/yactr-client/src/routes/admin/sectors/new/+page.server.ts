import { getAllAreas, uploadImage, createSector, type SectorImageRequestData } from "$lib/api";
import { fail, redirect } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import { zLineString, zPoint, zPolygon, zSectorRequestData } from "$lib/api/generated/zod.gen";
import { z } from "zod";
import { superValidate, withFiles } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";


// Required due to the way the generated zod schema constructs with `nullish` fields.
// TODO: Fix generated zod schema to use `optional` fields, might require an API change to
// use a specific request for creation.
const sectorRequestWithImages = zSectorRequestData.extend({
  name: z.string().min(1, { message: "Name is required" }),
  area_id: z.uuidv7().refine(id => id !== undefined, { message: "Area is required" }),
  primary_sector_image_id: z.string().optional(),
  primary_sector_image: z.instanceof(File).optional()
    .refine((file) => file?.size !== undefined && file?.size > 0, { message: "Primary sector image is required" }),
  sector_images: z.array(
    z.object({
      order: z.number().refine(order => order >= 0, { message: "Order must be greater than or equal to 0" }),
      image: z.instanceof(File).refine((file) => file?.size !== undefined && file?.size > 0, { message: "Image is required" })
    })
  ).optional(),
  entry_point: zPoint.optional(),
  recommended_parking_location: zPoint.optional(),
  approach_path: zLineString.optional(),
  sector_area: zPolygon.optional(),
});

export const load: PageServerLoad = async () => {
  const { data: areas } = await getAllAreas();
  const form = await superValidate(zod4(sectorRequestWithImages));

  return { areas, form };
}

export const actions = {
  default: async ({ request }) => {
    const form = await superValidate(request, zod4(sectorRequestWithImages));

    if (!form.valid) {
      console.log("form is not valid");
      console.dir(form, { depth: 10 })
      return fail(422, withFiles({ form }));
    }

    const { data: primary_sector_image_data, error: psi_error, response: psi_response } = await uploadImage({
      body: { image: form.data.primary_sector_image }
    });

    if (!psi_response.ok) {
      console.log("primary sector image upload failed");
      console.dir(psi_error);
      return fail(424, withFiles({ form, error: psi_error }));
    }
    
    const other_sector_images: SectorImageRequestData[] = [];

    for (const sector_image_with_order of form.data.sector_images ?? []) {
      const { data: other_sector_image_data, error: osi_error, response: osi_response } = await uploadImage({
        body: { image: sector_image_with_order.image }
      });

      if (!osi_response.ok) {
        console.log("other sector image upload failed");
        console.dir(osi_error);
        return fail(424, { form, error: { message: `Failed to upload sector image, order nr: ${sector_image_with_order.order}` } })
      }
      
      if (other_sector_image_data?.image_id) {
        other_sector_images.push({
          image_id: other_sector_image_data.image_id,
          order: sector_image_with_order.order
        });
      }
    }

    if (other_sector_images.length !== form.data.sector_images?.length) {
      console.log("unexpected number of sector images uploaded");
      console.dir(other_sector_images);
      console.dir(form.data.sector_images);
      return fail(424, { form, error: { message: `Unexpected number of sector images uploaded` } })
    }

    const { error: create_sector_error, response: create_sector_response } = await createSector({
      body: {
        name: form.data.name,
        area_id: form.data.area_id,
        primary_sector_image_id: primary_sector_image_data?.image_id,
        sector_images: other_sector_images,
        entry_point: form.data.entry_point,
        recommended_parking_location: form.data.recommended_parking_location ?? undefined,
        approach_path: form.data.approach_path ?? undefined,
        sector_area: form.data.sector_area,
      }
    });

    if (!create_sector_response.ok) {
      console.log("create sector failed");
      console.dir(create_sector_error);
      return fail(422, withFiles({ form, error: create_sector_error }));
    }

    return redirect(303, "/admin/sectors");
  }
} satisfies Actions;

