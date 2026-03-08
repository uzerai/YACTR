import {
  getAllAreas,
  getSectorById,
} from "$lib/api";
import { error } from "@sveltejs/kit";
import type { Actions, PageServerLoad } from "./$types";
import { zod4 } from "sveltekit-superforms/adapters";
import { superValidate } from "sveltekit-superforms";
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

  const { data: areas } = await getAllAreas();

  return {
    form,
    sector,
    areas
  }
}

export const actions = {
  default: async ({ request, params }) => {
    // const data = await request.formData();

    // const primary_sector_image = data.get("primary_sector_image") as File | undefined;
    // const primary_sector_image_upload = primary_sector_image && uploadImage({
    //   body: { image: primary_sector_image }
    // })

    // const other_sector_images = data.getAll("sector_images");
    // const other_sector_images_uploads = other_sector_images.map(image => uploadImage({
    //   body: { image: image as File }
    // }));

    // const { data: primary_sector_image_data } = primary_sector_image_upload ? await primary_sector_image_upload : {}
    // const other_images_data = await Promise.all(other_sector_images_uploads ?? []);
    // const other_images_ids = other_images_data.map(({ data }, index) => ({
    //   image_id: data?.image_id,
    //   order: index
    // }));

    // const sectorAreaBoundary = JSON.parse(data.get("sector_area")!.toString()) as Coordinate[][][];
    // const polygonCoordinates = sectorAreaBoundary?.[0]?.[0]?.map(coord => [...coord, 0.0]) ?? [];
    // if (polygonCoordinates.length > 0) {
    //   polygonCoordinates.push(polygonCoordinates.at(0)!);
    // }

    // const { response, error } = await updateSector({
    //   path: {
    //     sector_id: params.sector_id!
    //   },
    //   body: {
    //     name: data.get("name")!.toString(),
    //     area_id: data.get("area_id")!.toString(),
    //     primary_sector_image_id: primary_sector_image_data?.image_id,
    //     sector_images: other_images_ids,
    //     entry_point: {
    //       type: "Point",
    //       coordinates: JSON.parse(data.get("entry_point")!.toString())
    //     },
    //     recommended_parking_location: {
    //       type: "Point",
    //       coordinates: JSON.parse(data.get("recommended_parking_location")!.toString())
    //     },
    //     sector_area: {
    //       type: "Polygon",
    //       coordinates: [polygonCoordinates]
    //     },
    //     approach_path: {
    //       type: "LineString",
    //       coordinates: JSON.parse(data.get("approach_path")!.toString())
    //     }
    //   }
    // });


    // if (!response.ok) {
    //   return fail(422, { error });
    // }
  }
} satisfies Actions;

