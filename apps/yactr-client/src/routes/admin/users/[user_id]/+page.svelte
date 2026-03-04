<script lang="ts">
	import type { PageProps } from './$types';
	let { data }: PageProps = $props();
</script>

<div class="flex flex-col gap-4">
	<h1 class="text-2xl font-bold">User ({data.user_id})</h1>
	<p>Read-only placeholder. API supports only /users/me for details.</p>
</div>
