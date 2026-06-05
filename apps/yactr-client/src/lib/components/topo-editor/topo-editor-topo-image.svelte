<script lang="ts">
	import { getTopoEditorViewport } from './topo-editor-context.svelte';

	let { src }: { src: string | File | undefined } = $props();

	const viewport = getTopoEditorViewport();

	let imageElement = $state<HTMLImageElement>();
	let canvasWidth = $state<number>();
	let canvasHeight = $state<number>();

	let objectUrl = $state<string>();

	$effect(() => {
		if (!src || typeof src === 'string') {
			objectUrl = undefined;
			return;
		}

		const url = URL.createObjectURL(src);
		objectUrl = url;

		return () => {
			URL.revokeObjectURL(url);
		};
	});

	let imageUrl = $derived(typeof src === 'string' ? src : objectUrl);

	$effect(() => {
		if (!src) {
			viewport.setCanvasSize(undefined, undefined);
			viewport.setNaturalSize(undefined, undefined);
		}
	});

	$effect(() => {
		viewport.setCanvasSize(canvasWidth, canvasHeight);
	});
</script>

{#if imageUrl}
	<img
		alt="Background for route drawing"
		src={imageUrl}
		class="object-fit pointer-events-none absolute top-0 right-0 bottom-0 left-0 select-none"
		bind:this={imageElement}
		bind:clientWidth={canvasWidth}
		bind:clientHeight={canvasHeight}
		onload={() => {
			if (!imageElement) return;
			viewport.setNaturalSize(imageElement.naturalWidth, imageElement.naturalHeight);
		}}
	/>
{/if}
