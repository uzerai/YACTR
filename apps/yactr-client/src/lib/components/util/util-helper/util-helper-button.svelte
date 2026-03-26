<script lang="ts">
  import { cn } from "$lib/utils.js";
  import * as Popover from "$lib/components/ui/popover/index.js";
  import { buttonVariants } from "$lib/components/ui/button";
	import { Label } from "$lib/components/ui/label";
	import { Switch } from "$lib/components/ui/switch";
	import { superDebugStore } from "../super-debug-display/super-debug-store.ts";
	import SuperDebug from "sveltekit-superforms";
  let { class: className } = $props();
</script>

<Popover.Root>
  <Popover.Trigger class={cn(buttonVariants({ variant: "outline" }), className)}
    >Debug</Popover.Trigger
  >
  <Popover.Content align="end" interactOutsideBehavior="ignore">
    {#if $superDebugStore.form}
      <SuperDebug data={$superDebugStore.form} />
    {/if}
    
    <Label>Enable super debug</Label>
    <Switch bind:checked={$superDebugStore.enabled}/>
  </Popover.Content>
</Popover.Root>