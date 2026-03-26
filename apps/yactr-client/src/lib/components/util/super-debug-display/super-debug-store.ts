import { writable } from 'svelte/store';

export interface SuperDebugStore {
  enabled: boolean;
  form: unknown;
}

export const superDebugStore = writable<SuperDebugStore>({
  enabled: false,
  form: undefined
});
