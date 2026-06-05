<script lang="ts">
	import { RouteManagementForm } from '$lib/components/forms';
	import type { PageProps } from './$types';
	import * as Card from '$lib/components/ui/card';

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-6 p-4 max-w-7xl mx-auto">
	<h1 class="text-4xl">New Route</h1>
	<Card.Root>
		<Card.Content>
			<RouteManagementForm sectors={data.sectors!} data={data.form!} />
		</Card.Content>
	</Card.Root>
</div>
