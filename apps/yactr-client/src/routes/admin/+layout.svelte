<script lang="ts">
	import {
		Sidebar,
		SidebarGroup,
		SidebarItem,
		SidebarButton,
		uiHelpers,
		SidebarBrand,

		DarkMode

	} from 'flowbite-svelte';
	import { page } from '$app/state';

	let { children } = $props();

	/// Navigation bar logic.
	let activeUrl = $state(page.url.pathname);
	const navItemClass = 'flex-1 ms-3 whitespace-nowrap capitalize';
	const navSidebar = uiHelpers();
	let navOpen = $state(false);
	const closeNavigation = navSidebar.close;

	$effect(() => {
		navOpen = navSidebar.isOpen;
		activeUrl = page.url.pathname;
	});

	const site = {
		name: 'YACTR',
		href: '/admin'
	};

	const locationRoutes = ['areas', 'sectors', 'routes', 'pitches', 'ascents'];
	const organizationRoutes = ['organizations', 'teams', 'users'];
</script>

<div class="border-b md:hidden">
	<SidebarButton onclick={navSidebar.toggle} class="mb-2" />
</div>
<div class="relative">
	<Sidebar
		{activeUrl}
		backdrop={false}
		isOpen={navOpen}
		closeSidebar={closeNavigation}
		class="z-50 ml-4 md:ml-0 md:min-h-full"
		position="fixed"
	>
		<SidebarBrand {site} class="justify-center" classes={{ img: 'hidden' }} />
		<SidebarGroup class="pb-6">
			<SidebarItem href={`/admin`} label="Dashboard" class={navItemClass} />
		</SidebarGroup>
		<SidebarGroup class="flex flex-col gap-1 pb-6">
			{#each locationRoutes as route}
				<SidebarItem href={`/admin/${route}`} label={route} class={navItemClass} />
			{/each}
		</SidebarGroup>
		<SidebarGroup>
			{#each organizationRoutes as route}
				<SidebarItem href={`/admin/${route}`} label={route} class={navItemClass} />
			{/each}
		</SidebarGroup>
		<SidebarGroup>
			<DarkMode />
		</SidebarGroup>
	</Sidebar>

	<main class="min-h-dvh md:ml-64 dark:bg-neutral-900">
		<div class="relative p-4">
			{@render children?.()}
		</div>
	</main>
</div>
