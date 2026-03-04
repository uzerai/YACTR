<script lang="ts">
	import type { PageProps } from './$types';
	let { data, params }: PageProps = $props();

	let name = $state('');
	let type = $state('Sport');
	let description = $state('');

	$effect(() => {
		name = data.pitch?.name ?? '';
		type = data.pitch?.type ?? 'Sport';
		description = data.pitch?.description ?? '';
	});
</script>

<div class="flex max-w-xl flex-col gap-4">
	<h1 class="text-2xl font-bold">Edit Pitch ({params.pitch_id})</h1>
	<form method="post" class="flex flex-col gap-4">
		<div class="flex flex-col gap-1">
			<label class="font-semibold" for="name">Name</label>
			<input name="name" bind:value={name} />
		</div>
		<div class="flex flex-col gap-1">
			<label class="font-semibold" for="type">Type</label>
			<select name="type" bind:value={type} class="rounded border px-2 py-1">
				<option>Sport</option>
				<option>Traditional</option>
				<option>Boulder</option>
				<option>Mixed</option>
				<option>Aid</option>
			</select>
		</div>
		<div class="flex flex-col gap-1">
			<label class="font-semibold" for="description">Description</label>
			<textarea name="description" bind:value={description}></textarea>
		</div>
		<div class="flex justify-end">
			<button type="submit" class="rounded-md bg-green-500 px-4 py-2 text-antiflash-white"
				>Save</button
			>
		</div>
	</form>
</div>
