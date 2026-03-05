import { client } from "$lib/api/generated/client.gen";
import type { Handle } from "@sveltejs/kit";

/// This hook sets the @hey-api/client headers for authorization, but must always be called after the $lib/auth.ts hook.
export const authorizedClientHook: Handle = async ({ event, resolve }) => {
  const session = await event.locals.auth();

  if (session) {
    console.debug("Setting @hey-api/client headers for authorization");

    client.setConfig({
      headers: {
        Authorization: `Bearer ${session.access_token}`
      }
    });
  }

  return resolve(event);
};