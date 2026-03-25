<script lang="ts">
	import SectorManagementForm from '../sector-management-form.svelte';
	import * as Card from '$lib/components/ui/card';
	import { m } from '$lib/paraglide/messages.js';

	let { data } = $props();
</script>

<div class="mx-auto flex max-w-7xl flex-col gap-6 p-4">
	<h1 class="text-4xl">{m.admin_sectors_new_title()}</h1>
	<Card.Root>
		<Card.Content>
			<SectorManagementForm data={data.form} areas={data.areas ?? []} />
		</Card.Content>
	</Card.Root>
</div>
