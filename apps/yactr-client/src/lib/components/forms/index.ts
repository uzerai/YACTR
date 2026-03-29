import { zPoint, zRouteRequestData, zRouteResponse, zSectorRequestData, zSectorResponse } from "$lib/api/generated/zod.gen";
import SectorManagementForm from "./sector-management-form.svelte";
import AreaManagementForm from "./area-management-form.svelte";
import RouteManagementForm from "./route-management-form.svelte";
import PitchManagementForm from "./pitch-management-form.svelte";
import z from "zod";

/**
 * This is a tailored merged object for creation and update.
 * 
 * It contains the fields that are returned from zSectorResponse and zSectorRequestData, since it serves as
 * both the request and response schema for a form for managing sectors.
 * 
 * Additionally it contains the fields that the user can input for creation (such as direct File uploads).
 * 
 * Only really necessary because of the way superforms handles file uploads, and wanting to avoid uploading the 
 * files to the API before validation on the client-side server.
 */
const sectorManagementFormDto = zSectorResponse
  .extend(zSectorRequestData.shape)
  .extend({
    created_at: z.string().optional(),
    updated_at: z.string().optional(),
    entry_point: zPoint,
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

const routeManagementFormDto = zRouteResponse
  .extend(zRouteRequestData.shape)
  .extend({
    is_multipitch: z.boolean().optional(),
    topo_image: z.instanceof(File).optional(), // file holder for route topo image
    topo_image_overlay: z.instanceof(File).optional(), // file holder for svg overlay on route topo
    sector_topo_image_overlay: z.instanceof(File).optional(), // file holder for svg overlay on sector topo
    created_at: z.string().optional(),
    updated_at: z.string().optional(),
  });

export {
  sectorManagementFormDto,
  SectorManagementForm,
  AreaManagementForm,
  routeManagementFormDto,
  RouteManagementForm,
  PitchManagementForm,
}