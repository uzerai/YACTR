<script lang="ts">
	import type { PageProps } from './$types';
	import AreaManagementForm from '../area_management_form.svelte';
	import * as Card from '$lib/components/ui/card';

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-6 p-4 max-w-7xl mx-auto">
	<h1 class="text-4xl">{data.form.data.name}</h1>

	<Card.Root>
		<Card.Content>
			<AreaManagementForm data={data.form} />
		</Card.Content>
	</Card.Root>
</div>
