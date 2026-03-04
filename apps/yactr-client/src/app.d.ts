import type { YactrDataModelAuthenticationUser, client } from '$lib/api';
import { YACTR_BASE_API_URL } from '$env/static/private';

// See https://svelte.dev/docs/kit/types#app.d.ts
// for information about these interfaces

/// <reference types="@auth/sveltekit" />

declare global {
  namespace App {
    // interface Error {}
    interface Locals {
      user(): Promise<YactrDataModelAuthenticationUser | undefined>;
    }
    // interface PageData {}
    // interface PageState {}
    // interface Platform {}
  }
}


client.setConfig({
  baseUrl: YACTR_BASE_API_URL,
});

export { };
