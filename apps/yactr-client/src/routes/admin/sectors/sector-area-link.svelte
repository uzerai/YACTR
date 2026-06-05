<script lang="ts">
	let { areaId }: { areaId: string } = $props();
</script>

<a href={`/admin/areas/${areaId}`} class="text-primary underline underline-offset-2">{areaId}</a>
