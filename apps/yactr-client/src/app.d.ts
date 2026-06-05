import type { User } from '$lib/api';
import type { AuthSession } from '$lib/auth';

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

export { };
