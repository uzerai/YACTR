<script lang="ts">
	import AreaManagementForm from '../area_management_form.svelte';
	import * as Card from '$lib/components/ui/card';

	let { data } = $props();
</script>

<div class="flex flex-col gap-6 p-4 max-w-7xl mx-auto">
	<h1 class="text-4xl">New Area</h1>
	<Card.Root>
		<Card.Content>
			<AreaManagementForm data={data.form} />
		</Card.Content>
	</Card.Root>
</div>