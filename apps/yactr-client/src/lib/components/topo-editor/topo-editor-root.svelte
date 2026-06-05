<script lang="ts">
	import type { Snippet } from 'svelte';
	import { TopoEditorViewport, setTopoEditorViewport } from './topo-editor-context.svelte';

	let {
		class: className,
		children
	}: {
		class?: string;
		children?: Snippet;
	} = $props();

	const viewport = new TopoEditorViewport();
	setTopoEditorViewport(viewport);
</script>

<div class={['relative overflow-hidden rounded-lg border', className]}>
	{@render children?.()}
</div>
