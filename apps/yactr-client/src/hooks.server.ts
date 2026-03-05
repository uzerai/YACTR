import { sequence } from '@sveltejs/kit/hooks';
import { type Handle } from '@sveltejs/kit';
import { getAPIUserHook, handle as handleAuth } from '$lib/auth';
import { authorizedClientHook } from '$lib/api/authorized_client_hook';
import { paraglideMiddleware } from '$lib/paraglide/server';

// Useful for debugging cookies if needed.
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const logCookies: Handle = async ({ event, resolve }) => {
  console.dir(event.cookies.getAll());
  return resolve(event);
};

const handleParaglide: Handle = ({ event, resolve }) =>
	paraglideMiddleware(event.request, ({ request, locale }) => {
		event.request = request;

		return resolve(event, {
			transformPageChunk: ({ html }) => html.replace('%paraglide.lang%', locale)
		});
	});

export const handle = sequence(handleAuth, authorizedClientHook, getAPIUserHook, handleParaglide);