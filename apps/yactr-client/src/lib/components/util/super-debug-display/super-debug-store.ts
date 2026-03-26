import type { SuperForm } from 'sveltekit-superforms';
import { writable } from 'svelte/store';

export interface SuperDebugStore {
  enabled: boolean;
  form: SuperForm<Record<string, unknown>, unknown> | undefined;
}

export const superDebugStore = writable<SuperDebugStore>({
  enabled: false,
  form: undefined
});
