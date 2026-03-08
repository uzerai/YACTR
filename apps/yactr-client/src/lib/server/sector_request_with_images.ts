// Required due to the way the generated zod schema constructs with `nullish` fields.
// TODO: Fix generated zod schema to use `optional` fields, might require an API change.

import { zLineString, zPoint, zPolygon, zSectorResponse } from "$lib/api/generated/zod.gen";
import z from "zod";

// This is a tailored merged request for creation and update.
// It contains the fields that are returned from zSectorResponse as `optional()` fields,
// in addition to containing the fields that the user can input for creation (such as direct File uploads).
// Only really necessary because of the way superforms handles file uploads, and wanting to avoid uploading the 
// files to the API before validation on the client-side server.
export const sectorRequestWithImages = zSectorResponse.extend({
  name: z.string().min(1, { message: "Name is required" }),
  area_id: z.uuidv7().refine(id => id !== undefined, { message: "Area is required" }),
  primary_sector_image_id: z.string().optional(),
  primary_sector_image_url: z.string().optional(),
  sector_images: z.array(
    z.object({
      order: z.number().refine(order => order >= 0, { message: "Order must be greater than or equal to 0" }),
      is_primary: z.boolean().optional(),
      image: z.instanceof(File).optional().refine((file) => file?.size !== undefined && file?.size > 0, { message: "Image is required" }),
      image_id: z.string().optional(),
      image_url: z.string().optional(),
    }).refine(img => img.image_id !== undefined || img.image !== undefined, { message: "Image or image_id is required" })
  ),
  entry_point: zPoint.optional(),
  recommended_parking_location: zPoint.optional(),
  approach_path: zLineString.optional(),
  sector_area: zPolygon
});