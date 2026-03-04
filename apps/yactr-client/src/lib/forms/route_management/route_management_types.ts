import type { YactrDataModelClimbingClimbingType } from "$lib/api";

// Strings _CAN_ be json serialized forms of more complex objects
// TODO: Add type-hinting to json-serialized strings for automatic deserialization.
export interface RouteManagementFormType {
  sector_id: string;
  name: string;
  type: YactrDataModelClimbingClimbingType;
  height?: number;
  description?: string;
  grade?: string;
  number_of_bolts?: number;
  bolter_name?: string;
  first_ascent_climber_name?: string;
  route_image?: File;
  route_image_svg_overlay?: File;
  topo_line_points?: string;
  sector_topo_image_id?: string;
  sector_image_svg_overlay?: File;
  sector_topo_line_points?: string;
  pitches?: string;
}