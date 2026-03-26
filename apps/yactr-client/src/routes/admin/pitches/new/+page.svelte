<script lang="ts">
	import { PitchManagementForm } from '$lib/components/forms';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	let { data }: PageProps = $props();
</script>

<div class="mx-auto flex max-w-7xl flex-col gap-6 p-4">
	<h1 class="text-4xl">New Pitch</h1>
	<Card.Root>
		<Card.Content>
			<PitchManagementForm sectors={data.sectors ?? []} showSectorSelect includeGrade />
		</Card.Content>
	</Card.Root>
</div>
