<script lang="ts">
	import type { PageProps } from './$types';
	let { data }: PageProps = $props();

	let organization_id = $state('');
	let name = $state('');
</script>

<div class="flex max-w-md flex-col gap-4">
	<h1 class="text-2xl font-bold">New Team</h1>
	<form method="post" class="flex flex-col gap-4">
		<div class="flex flex-col gap-1">
			<label class="font-semibold">Organization</label>
			<input type="hidden" name="organization_id" bind:value={organization_id} />
			<select class="rounded border px-2 py-1" bind:value={organization_id}>
				<option value="" disabled>Select organization</option>
				{#each data.organizations ?? [] as org}
					<option value={org.id}>{org.name}</option>
				{/each}
			</select>
		</div>
		<div class="flex flex-col gap-1">
			<label class="font-semibold">Name</label>
			<input name="name" bind:value={name} />
		</div>
		<div class="flex justify-end">
			<button type="submit" class="rounded-md bg-green-500 px-4 py-2 text-antiflash-white"
				>Save</button
			>
		</div>
	</form>
</div>
