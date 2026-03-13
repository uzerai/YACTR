import { zRouteRequestData, zRouteResponse } from "$lib/api/generated/zod.gen";

export const routeManagementFormDto = zRouteResponse
  .extend(zRouteRequestData);