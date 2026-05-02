<script lang="ts">
	import type { GetAllSectorsResponseItem } from '$lib/api';
	import { Input } from '$lib/components/ui/input';
	import { Textarea } from '$lib/components/ui/textarea';
	import { Button } from '$lib/components/ui/button';
	import * as NativeSelect from '$lib/components/ui/native-select';
	import { Label } from '$lib/components/ui/label';

	let {
		sectors = [] as GetAllSectorsResponseItem[],
		showSectorSelect = false,
		initialSectorId = '',
		initialName = '',
		initialType = 'Sport',
		initialDescription = '',
		initialGrade = '',
		includeGrade = false
	}: {
		sectors?: GetAllSectorsResponseItem[];
		showSectorSelect?: boolean;
		initialSectorId?: string;
		initialName?: string;
		initialType?: string;
		initialDescription?: string;
		initialGrade?: string;
		includeGrade?: boolean;
	} = $props();
</script>

<form method="post" class="flex flex-col gap-4">
	{#if showSectorSelect}
		<div class="flex flex-col gap-1">
			<Label for="sector_id">Sector</Label>
			<NativeSelect.Root id="sector_id" name="sector_id" value={initialSectorId} required>
				<NativeSelect.Option value="" disabled>Select sector</NativeSelect.Option>
				{#each sectors as sector (sector.id)}
					<NativeSelect.Option value={sector.id}>{sector.name}</NativeSelect.Option>
				{/each}
			</NativeSelect.Root>
		</div>
	{/if}

	<div class="flex flex-col gap-1">
		<Label for="name">Name</Label>
		<Input id="name" name="name" value={initialName} />
	</div>

	<div class="flex flex-col gap-1">
		<Label for="type">Type</Label>
		<NativeSelect.Root id="type" name="type" value={initialType}>
			<NativeSelect.Option value="Sport">Sport</NativeSelect.Option>
			<NativeSelect.Option value="Traditional">Traditional</NativeSelect.Option>
			<NativeSelect.Option value="Boulder">Boulder</NativeSelect.Option>
			<NativeSelect.Option value="Mixed">Mixed</NativeSelect.Option>
			<NativeSelect.Option value="Aid">Aid</NativeSelect.Option>
		</NativeSelect.Root>
	</div>

	<div class="flex flex-col gap-1">
		<Label for="description">Description</Label>
		<Textarea id="description" name="description" value={initialDescription} rows={4} />
	</div>

	{#if includeGrade}
		<div class="flex flex-col gap-1">
			<Label for="grade">Grade</Label>
			<Input id="grade" name="grade" value={initialGrade} />
		</div>
	{/if}

	<div class="mt-2 flex justify-end">
		<Button type="submit">Save</Button>
	</div>
</form>
