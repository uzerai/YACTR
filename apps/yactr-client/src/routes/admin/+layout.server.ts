import { error, redirect } from "@sveltejs/kit";
import type { LayoutServerLoad } from "./$types";

export const load: LayoutServerLoad = async (event) => {
  const session = await event.locals.auth();
  const user = await event.locals.user();

  if (!session || !user) {
    throw redirect(302, `/sign-in?callbackUrl=${event.url.pathname}`);
  }

  if ((user?.admin_permissions?.length ?? 0) < 1) throw error(403, "You are not authorized to access this page");

  return {
    session,
    user
  }
}