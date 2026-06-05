import type { CreateClientConfig } from '$lib/api/generated/client.gen';
import { YACTR_BASE_API_URL } from '$env/static/private';

export const createClientConfig: CreateClientConfig = (config) => ({
  ...config,
  baseUrl: YACTR_BASE_API_URL,
});