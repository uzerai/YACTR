<script lang="ts">
	import * as Sidebar from '$lib/components/ui/sidebar';
	import { MapsEditingIcon, PinLocation02Icon, CurvyUpDownDirectionIcon, AutoConversationsIcon } from '@hugeicons/core-free-icons';
	import { HugeiconsIcon as Icon } from '@hugeicons/svelte';
	
	let { children } = $props();

	const site = {
		name: 'YACTR',
		href: '/admin'
	};

	const locationRoutes = ['areas', 'sectors', 'routes', 'pitches', 'ascents'];
	const organizationRoutes = ['organizations', 'teams', 'users'];

	const climbingNavItems = [
    {
      title: "Areas",
      url: "admin/areas",
      icon: MapsEditingIcon,
    },
		{
			title: "Sectors",
			url: "admin/sectors",
			icon: PinLocation02Icon,
		},
		{
			title: "Routes",
			url: "admin/routes",
			icon: CurvyUpDownDirectionIcon,
		},
		{
			title: "Pitches",
			url: "admin/pitches",
			icon: AutoConversationsIcon,
		}
  ];
</script>

<Sidebar.Provider>
  <Sidebar.Root>
    <Sidebar.Header>
			<div class="flex items-center justify-center gap-2">
				<span>Y A C T R</span><span>|</span><span class="text-xs text-gray-500">Admin</span>
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
  </Sidebar.Root>
  <main>
	<Sidebar.Trigger />
    {@render children()}
  </main>
</Sidebar.Provider>
