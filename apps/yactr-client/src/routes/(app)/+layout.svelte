<script lang="ts">
	import '../../app.css';
	import favicon from '$lib/assets/favicon.svg';
	import * as NavigationMenu from '$lib/components/ui/navigation-menu';
	import { resolve } from '$app/paths';
 
	let { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<NavigationMenu.Root viewport={false} class="mx-auto mt-6 bg-background/80 rounded-lg py-2 px-4 sticky">
  <NavigationMenu.List class="flex gap-6">
    <NavigationMenu.Item>
			<NavigationMenu.Link href={resolve('/')}>Home</NavigationMenu.Link>
    </NavigationMenu.Item>
		<NavigationMenu.Item>
			<NavigationMenu.Link href={resolve('/map')}>Map</NavigationMenu.Link>
    </NavigationMenu.Item>
		<NavigationMenu.Item>
			<NavigationMenu.Link href={resolve('/')}>About</NavigationMenu.Link>
    </NavigationMenu.Item>
  </NavigationMenu.List>
</NavigationMenu.Root>


<main>
	{@render children?.()}
</main>
