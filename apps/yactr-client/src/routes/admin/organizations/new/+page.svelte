<script lang="ts">
	let name = $state('');
</script>

<div class="flex max-w-md flex-col gap-4">
	<h1 class="text-2xl font-bold">New Organization</h1>
	<form method="post" class="flex flex-col gap-4">
		<div class="flex flex-col gap-1">
			<label class="font-semibold" for="name">Name</label>
			<input name="name" bind:value={name} />
		</div>
		<div class="flex justify-end">
			<button type="submit" class="rounded-md bg-green-500 px-4 py-2 text-antiflash-white"
				>Save</button
			>
		</div>
	</form>
</div>
