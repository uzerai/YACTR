import { client } from "$lib/api/generated/client.gen";
import type { AuthSession } from "$lib/auth";
import { YACTR_BASE_API_URL } from "$env/static/private";
import type { RequestEvent } from "@sveltejs/kit";

export async function getAccessToken(event: RequestEvent): Promise<string | null> {
  const session = await event.locals.auth();
  return session?.access_token ?? null;
}

export function configureAuthorizedClient(session: AuthSession | null): void {
  client.setConfig({
    baseUrl: YACTR_BASE_API_URL,
    ...(session ? { auth: () => session.access_token } : {})
  });
}
