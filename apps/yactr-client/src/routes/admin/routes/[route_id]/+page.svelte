<script lang="ts">
	import type { PageProps } from './$types';
	import { RouteManagementForm } from '$lib/components/forms';
	import * as Card from '$lib/components/ui/card';

	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-6 p-4 max-w-7xl mx-auto">
	<h1 class="text-4xl">{data.route?.name}</h1>

	<Card.Root>
		<Card.Content>
			<RouteManagementForm data={data.form!} sectors={data.sectors!} />
		</Card.Content>
	</Card.Root>
</div>
