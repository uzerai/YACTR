<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { m } from '$lib/paraglide/messages.js';

	let { sectorId }: { sectorId: string } = $props();
</script>

<div class="flex gap-1">
	<Button href={`/admin/sectors/${sectorId}`} variant="outline">{m.admin_sectors_edit()}</Button>
	<form method="post" action="?/delete">
		<input type="hidden" name="sector_id" value={sectorId} />
		<Button type="submit" variant="destructive">{m.admin_sectors_delete()}</Button>
	</form>
</div>
