import { zSectorRequestData, zSectorResponse } from "$lib/api/generated/zod.gen";
import z from "zod";

/**
 * This is a tailored merged object for creation and update.
 * It contains the fields that are returned from zSectorResponse and zSectorRequestData, since it serves as
 * both the request and response schema for a form for managing sectors.
 * Additionally it contains the fields that the user can input for creation (such as direct File uploads).
 * Only really necessary because of the way superforms handles file uploads, and wanting to avoid uploading the 
 * files to the API before validation on the client-side server.
 */
export const sectorManagementFormDto = zSectorResponse
  .extend(zSectorRequestData.shape)
  .extend({
    created_at: z.string().optional(),
    updated_at: z.string().optional(),
    sector_images: z.array(
      z.object({
        order: z.number().refine(order => order >= 0, { message: "Order must be greater than or equal to 0" }),
        is_primary: z.boolean().optional(),
        image: z.instanceof(File).optional().refine((file) => file?.size !== undefined && file?.size > 0, { message: "Image is required" }),
        image_id: z.string().optional(),
        image_url: z.string().optional(),
      }).refine(img => img.image_id !== undefined || img.image !== undefined, { message: "Image or image_id is required" })
    ),
});