<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { m } from '$lib/paraglide/messages.js';

	let { areaId }: { areaId: string } = $props();
</script>

<div class="flex gap-1">
	<Button href={`/admin/areas/${areaId}`} variant="outline">{m.admin_areas_edit()}</Button>
	<form method="post" action="?/delete">
		<input type="hidden" name="area_id" value={areaId} />
		<Button type="submit" variant="destructive">{m.admin_areas_delete()}</Button>
	</form>
</div>
