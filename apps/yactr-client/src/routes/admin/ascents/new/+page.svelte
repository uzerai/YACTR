<script lang="ts">
	let route_id = $state('');
	let type = $state(0);
	let completed_at = $state('');
</script>

<div class="flex flex-col gap-4">
	<h1 class="text-2xl font-bold">New Ascent</h1>
	<form method="post" class="flex max-w-md flex-col gap-4">
		<div class="flex flex-col gap-1">
			<label for="route_id" class="font-semibold">Route Id</label>
			<input id="route_id" name="route_id" bind:value={route_id} />
		</div>
		<div class="flex flex-col gap-1">
			<label for="type" class="font-semibold">Type</label>
			<input id="type" name="type" type="number" min="0" max="7" bind:value={type} />
		</div>
		<div class="flex flex-col gap-1">
			<label for="completed_at" class="font-semibold">Completed At (ISO)</label>
			<input id="completed_at" name="completed_at" bind:value={completed_at} />
		</div>
		<div class="flex justify-end">
			<button type="submit" class="rounded-md bg-green-500 px-4 py-2 text-antiflash-white"
				>Save</button
			>
		</div>
	</form>
</div>
