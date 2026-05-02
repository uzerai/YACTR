<script lang="ts">
	import * as Sidebar from '$lib/components/ui/sidebar';
	import DarkmodeToggle from '$lib/components/darkmode-toggle/darkmode-toggle.svelte';
	import { MapsEditingIcon, PinLocation02Icon, CurvyUpDownDirectionIcon, AutoConversationsIcon } from '@hugeicons/core-free-icons';
	import { HugeiconsIcon as Icon } from '@hugeicons/svelte';
	import { m } from '$lib/paraglide/messages.js';
	import { resolve } from '$app/paths';
	
	let { children } = $props();

	const climbingNavItems = [
    {
      title: m.admin_layout_nav_areas(),
      url: "/admin/areas",
      icon: MapsEditingIcon,
    },
		{
			title: m.admin_layout_nav_sectors(),
			url: "/admin/sectors",
			icon: PinLocation02Icon,
		},
		{
			title: m.admin_layout_nav_routes(),
			url: "/admin/routes",
			icon: CurvyUpDownDirectionIcon,
		},
		{
			title: m.admin_layout_nav_pitches(),
			url: "/admin/pitches",
			icon: AutoConversationsIcon,
		}
  ] as const;
</script>

<Sidebar.Provider>
  <Sidebar.Root collapsible="icon">
    <Sidebar.Header>
			<a href={resolve('/')} class="flex items-center justify-center gap-2">
				<span>Y A C T R</span>
			</a>
		</Sidebar.Header>
		<Sidebar.Content>
			<Sidebar.Group>
				<Sidebar.GroupLabel>
					{m.admin_layout_group_climbing()}
				</Sidebar.GroupLabel>
				<Sidebar.GroupContent>
					<Sidebar.Menu>
						{#each climbingNavItems as item (item.title)}
							<Sidebar.MenuItem>
								<Sidebar.MenuButton>
									{#snippet child({ props })}
										<a href={resolve(item.url)} {...props}>
											<Icon icon={item.icon} />
											<span>{item.title}</span>
										</a>
									{/snippet}
								</Sidebar.MenuButton>
							</Sidebar.MenuItem>
						{/each}
					</Sidebar.Menu>
				</Sidebar.GroupContent>
			</Sidebar.Group>
			<Sidebar.Separator />
			<Sidebar.Group>
				<Sidebar.GroupLabel>
					{m.admin_layout_group_administration()}
				</Sidebar.GroupLabel>
			</Sidebar.Group>
		</Sidebar.Content>
		<Sidebar.Footer>
			<DarkmodeToggle />
		</Sidebar.Footer>
		<Sidebar.Rail>
		</Sidebar.Rail>
  </Sidebar.Root>
  <main class="p-10 w-full relative">
    {@render children()}
  </main>
</Sidebar.Provider>
