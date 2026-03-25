<script lang="ts">
	import * as Sidebar from '$lib/components/ui/sidebar';
	import DarkmodeToggle from '$lib/components/darkmode-toggle/darkmode-toggle.svelte';
	import { MapsEditingIcon, PinLocation02Icon, CurvyUpDownDirectionIcon, AutoConversationsIcon } from '@hugeicons/core-free-icons';
	import { HugeiconsIcon as Icon } from '@hugeicons/svelte';
	
	let { children } = $props();

	const climbingNavItems = [
    {
      title: "Areas",
      url: "/admin/areas",
      icon: MapsEditingIcon,
    },
		{
			title: "Sectors",
			url: "/admin/sectors",
			icon: PinLocation02Icon,
		},
		{
			title: "Routes",
			url: "/admin/routes",
			icon: CurvyUpDownDirectionIcon,
		},
		{
			title: "Pitches",
			url: "/admin/pitches",
			icon: AutoConversationsIcon,
		}
  ];
</script>

<Sidebar.Provider>
  <Sidebar.Root collapsible="icon">
    <Sidebar.Header>
			<div class="flex items-center justify-center gap-2">
				<span>Y A C T R</span>
			</div>
		</Sidebar.Header>
		<Sidebar.Content>
			<Sidebar.Group>
				<Sidebar.GroupLabel>
					Climbing
				</Sidebar.GroupLabel>
				<Sidebar.GroupContent>
					<Sidebar.Menu>
						{#each climbingNavItems as item (item.title)}
							<Sidebar.MenuItem>
								<Sidebar.MenuButton>
									{#snippet child({ props })}
										<a href={item.url} {...props}>
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
					Administration
				</Sidebar.GroupLabel>
			</Sidebar.Group>
		</Sidebar.Content>
		<Sidebar.Footer>
			<DarkmodeToggle />
		</Sidebar.Footer>
		<Sidebar.Rail>
		</Sidebar.Rail>
  </Sidebar.Root>
  <main class="p-4 w-full relative">
    {@render children()}
  </main>
</Sidebar.Provider>
