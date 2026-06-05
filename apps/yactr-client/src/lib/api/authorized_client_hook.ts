import { configureAuthorizedClient, getAccessToken } from "$lib/api/authorized_api";
import type { Handle } from "@sveltejs/kit";

/// This hook sets the @hey-api/client headers for authorization, but must always be called after the $lib/auth.ts hook.
export const authorizedClientHook: Handle = async ({ event, resolve }) => {
  const token = await getAccessToken(event);
  configureAuthorizedClient(token ? { access_token: token } : null);

  return resolve(event);
};
