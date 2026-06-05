import { writable } from 'svelte/store';

export interface SuperDebugStore {
	enabled: boolean;
	form: unknown;
	sourceId: number | undefined;
}

export const superDebugStore = writable<SuperDebugStore>({
	enabled: false,
	form: undefined,
	sourceId: undefined
});
