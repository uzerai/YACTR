import { sequence } from '@sveltejs/kit/hooks';
import { error, redirect, type Handle } from '@sveltejs/kit';
import { getAPIUserHook, handle as handleAuth } from '$lib/auth';
import { authorizedClientHook } from '$lib/api/authorized_client_hook';
import { paraglideMiddleware } from '$lib/paraglide/server';

// Useful for debugging cookies if needed.
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const logCookies: Handle = async ({ event, resolve }) => {
  console.dir(event.cookies.getAll());
  return resolve(event);
};

const protectAdminRoutes: Handle = async ({ event, resolve }) => {
  // Only run auth checks for admin routes so "/" and other public routes are not required to authenticate
  if (!event.url.pathname.startsWith("/admin")) {
    return resolve(event);
  }

  const session = await event.locals.auth();
  const user = await event.locals.user();

  if (!session) throw redirect(302, `/auth/signin?callbackUrl=${event.url.pathname}`);
  if ((user?.admin_permissions?.length ?? 0) < 1) throw error(403, "You are not authorized to access this page");

  return resolve(event);
};

const handleParaglide: Handle = ({ event, resolve }) =>
	paraglideMiddleware(event.request, ({ request, locale }) => {
		event.request = request;

		return resolve(event, {
			transformPageChunk: ({ html }) => html.replace('%paraglide.lang%', locale)
		});
	});

export const handle = sequence(handleAuth, authorizedClientHook, getAPIUserHook, handleParaglide, protectAdminRoutes);