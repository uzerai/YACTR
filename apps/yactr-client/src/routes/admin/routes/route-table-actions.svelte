<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { m } from '$lib/paraglide/messages.js';

	let { routeId }: { routeId: string } = $props();
</script>

<div class="flex gap-1">
	<Button href={`/admin/routes/${routeId}`} variant="outline">{m.admin_routes_edit()}</Button>
	<form method="post" action="?/delete">
		<input type="hidden" name="route_id" value={routeId} />
		<Button type="submit" variant="destructive">{m.admin_routes_delete()}</Button>
	</form>
</div>
