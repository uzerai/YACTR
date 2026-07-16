<script lang="ts">
	import type { SortDirection } from '@tanstack/table-core';
	import ArrowUpIcon from '@lucide/svelte/icons/arrow-up';
	import ArrowDownIcon from '@lucide/svelte/icons/arrow-down';
	import ArrowUpDownIcon from '@lucide/svelte/icons/arrow-up-down';
	import { Button } from '$lib/components/ui/button';

	type Props = {
		label: string;
		sorted: false | SortDirection;
		onsort: () => void;
	};

	let { label, sorted, onsort }: Props = $props();
</script>

<Button variant="ghost" class="-ml-3 h-8" onclick={onsort}>
	{label}
	{#if sorted === 'asc'}
		<ArrowUpIcon class="size-4" />
	{:else if sorted === 'desc'}
		<ArrowDownIcon class="size-4" />
	{:else}
		<ArrowUpDownIcon class="size-4 opacity-50" />
	{/if}
</Button>
