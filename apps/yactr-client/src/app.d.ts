import type { User } from '$lib/api';
import { client } from '$lib/api/generated/client.gen';
import type { AuthSession } from '$lib/auth';
import { YACTR_BASE_API_URL } from '$env/static/private';

// See https://svelte.dev/docs/kit/types#app.d.ts
// for information about these interfaces

declare global {
  namespace App {
    // interface Error {}
    interface Locals {
      auth: () => Promise<AuthSession | null>;
      user(): Promise<User | undefined>;
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
