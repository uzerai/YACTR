<script lang="ts">
	import type { PageProps } from './$types';
	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-4">
	<h1 class="text-2xl font-bold">Team ({data.team_id})</h1>
	<p>Edit is not available via API; this is a placeholder page.</p>
</div>
