<script lang="ts">
	import AreaManagementForm from '../area-management-form.svelte';
	import * as Card from '$lib/components/ui/card';
	import { m } from '$lib/paraglide/messages.js';

	let { data } = $props();
</script>

<div class="flex flex-col gap-6 p-4 max-w-7xl mx-auto">
	<h1 class="text-4xl">{m.admin_areas_new_title()}</h1>
	<Card.Root>
		<Card.Content>
			<AreaManagementForm data={data.form} />
		</Card.Content>
	</Card.Root>
</div>