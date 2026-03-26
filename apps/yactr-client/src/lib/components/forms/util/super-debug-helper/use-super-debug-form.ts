import { onDestroy } from 'svelte';
import type { Readable } from 'svelte/store';
import { superDebugStore } from './super-debug-store';

let nextSourceId = 0;

export function useSuperDebugForm(formStore: Readable<unknown>) {
	const sourceId = ++nextSourceId;

	const unsubscribe = formStore.subscribe((value) => {
		superDebugStore.update((store) => ({
			...store,
			form: value,
			sourceId
		}));
	});

	onDestroy(() => {
		unsubscribe();
		superDebugStore.update((store) => {
			if (store.sourceId !== sourceId) {
				return store;
			}

			return {
				...store,
				form: undefined,
				sourceId: undefined
			};
		});
	});
}
