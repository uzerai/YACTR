import type { CreateClientConfig } from '$lib/api/generated/client.gen';

/**
 * This is the client config which will be used on the browser-client to 
 * proxy requests to the API, ensuring that we don't leak the API URL on the client, and
 * allows us to only do internal-network requests to the API from the yactr-client container.
 * 
 * see the `src/routes/api/[...path]/+server.ts` route for more details.
 * 
 * @param config 
 * @returns 
 */
export const createClientConfig: CreateClientConfig = (config) => ({
  ...config,
  baseUrl: '/api',
});
