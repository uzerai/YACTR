import { yactrEndpointsAreasGetAllAreas, yactrEndpointsImagesUploadImage, yactrEndpointsSectorsCreateSector } from "$lib/api";
import { fail, redirect } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import type { Coordinate } from "ol/coordinate";

export const load: PageServerLoad = async () => {
  const { data: areas } = await yactrEndpointsAreasGetAllAreas();

  return { areas };
}

export const actions = {
  default: async ({ request }) => {
    const data = await request.formData();

    // File handling first
    const primary_sector_image = data.get("primary_sector_image") as File | undefined;
    const primary_sector_image_upload = primary_sector_image && yactrEndpointsImagesUploadImage({
      body: { image: primary_sector_image }
    })

    const other_sector_images = data.getAll("sector_images");
    const other_sector_images_uploads = other_sector_images.map(image => yactrEndpointsImagesUploadImage({
      body: { image: image as File }
    }));

    const { data: primary_sector_image_data } = primary_sector_image_upload ? await primary_sector_image_upload : {}
    const other_images_data = await Promise.all(other_sector_images_uploads);
    const other_images_ids = other_images_data.map(({ data }, index) => ({
      image_id: data?.image_id,
      order: index
    }));

    const sectorAreaBoundary = JSON.parse(data.get("sector_area")!.toString()) as Coordinate[][][];
    const polygonCoordinates = sectorAreaBoundary?.[0]?.[0]?.map(coord => [...coord, 0.0]) ?? [];
    if (polygonCoordinates.length > 0) {
      polygonCoordinates.push(polygonCoordinates.at(0)!);
    }

    const { response, error } = await yactrEndpointsSectorsCreateSector({
      body: {
        name: data.get("name")!.toString(),
        area_id: data.get("area_id")!.toString(),
        primary_sector_image_id: primary_sector_image_data?.image_id,
        sector_images: other_images_ids,
        entry_point: {
          type: "Point",
          coordinates: JSON.parse(data.get("entry_point")!.toString())
        },
        recommended_parking_location: {
          type: "Point",
          coordinates: JSON.parse(data.get("recommended_parking_location")!.toString())
        },
        sector_area: {
          type: "Polygon",
          coordinates: [polygonCoordinates]
        },
        approach_path: {
          type: "LineString",
          coordinates: JSON.parse(data.get("approach_path")!.toString())
        }
      }
    });

    if (!response.ok) {
      return fail(422, { error });
    }

    return redirect(303, "/admin/sectors");
  }
} satisfies Actions;

