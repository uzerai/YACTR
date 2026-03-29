<script lang="ts">
	import type { PageProps } from './$types';
	import { SectorManagementForm } from '$lib/components/forms';
	import * as Card from '$lib/components/ui/card';
	import { Badge } from '$lib/components/ui/badge';
	import * as Tooltip from '$lib/components/ui/tooltip';
	import { m } from '$lib/paraglide/messages.js';
	import { HugeiconsIcon as Icon } from '@hugeicons/svelte';
	import { HelpCircleIcon } from '@hugeicons/core-free-icons';

	let { data }: PageProps = $props();
</script>

<div class="mx-auto flex max-w-7xl flex-col gap-6 p-4">
	<div class="flex flex-wrap items-center gap-2">
		<h1 class="text-4xl">{data?.sector?.name}</h1>
		<Tooltip.Root>
			<Tooltip.Trigger class="inline-flex items-center gap-1.5">
				<Badge variant="secondary">{m.admin_sectors_edit_badge()}</Badge>
				<Icon icon={HelpCircleIcon} class="text-muted-foreground size-4 shrink-0" />
			</Tooltip.Trigger>
			<Tooltip.Content>
				<p>{m.admin_sectors_edit_tooltip()}</p>
			</Tooltip.Content>
		</Tooltip.Root>
	</div>

	<Card.Root>
		<Card.Content>
			<SectorManagementForm data={data.form} areas={data.areas ?? []} />
		</Card.Content>
	</Card.Root>
</div>
