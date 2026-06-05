<script lang="ts">
	import { PitchManagementForm } from '$lib/components/forms';
	import * as Card from '$lib/components/ui/card';
	import type { PageProps } from './$types';
	let { data, params }: PageProps = $props();
</script>

<div class="mx-auto flex max-w-7xl flex-col gap-6 p-4">
	<h1 class="text-4xl">Edit Pitch ({params.pitch_id})</h1>
	<Card.Root>
		<Card.Content>
			<PitchManagementForm
				initialName={data.pitch?.name ?? ''}
				initialType={data.pitch?.type ?? 'Sport'}
				initialDescription={data.pitch?.description ?? ''}
			/>
		</Card.Content>
	</Card.Root>
</div>
