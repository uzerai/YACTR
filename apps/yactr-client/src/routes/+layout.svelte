<script lang="ts">
	import '../app.css';
	import favicon from '$lib/assets/favicon.svg';

	import { overrideItemIdKeyNameBeforeInitialisingDndZones } from "svelte-dnd-action";
	
	// Override the default item id key name for svelte-dnd-action to use "order" instead of "id" across
	// the entire application. We want to maintain using "order" since "id" is UUIDv7 across the entire application API.
	overrideItemIdKeyNameBeforeInitialisingDndZones("order");

	let { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

{@render children?.()}
