<script lang="ts">
	import { cn } from '$lib/utils.js';
	import { buttonVariants } from '$lib/components/ui/button';
	import { superDebugStore } from './super-debug-store';
	import SuperDebug from 'sveltekit-superforms';

	let { class: className } = $props();
	let open = $state(false);
</script>

<button
	type="button"
	class={cn(buttonVariants({ variant: 'outline' }), className)}
	onclick={() => (open = !open)}
>
	Superforms debug
</button>

{#if open}
	<div
		class="fixed bottom-20 right-6 z-40 flex w-md max-w-[calc(100vw-3rem)] flex-col gap-2 rounded-lg border bg-popover p-3 text-popover-foreground shadow-lg"
	>
		<div class="max-h-[70vh] overflow-y-auto pr-1">
			{#if $superDebugStore.form}
				<SuperDebug data={$superDebugStore.form} />
			{/if}
		</div>
	</div>
{/if}
