<script lang="ts">
	import type { ChangeEventHandler } from 'svelte/elements';
	import { dragHandle, dragHandleZone } from 'svelte-dnd-action';
	import { m } from '$lib/paraglide/messages.js';
	import { HugeiconsIcon } from '@hugeicons/svelte';
	import {
		Delete02Icon,
		DragDropVerticalIcon,
		StarIcon,
		StarOffIcon
	} from '@hugeicons/core-free-icons';

	type OrderableImageUploadItem = {
		order: number;
		is_primary?: boolean;
		image?: File;
		image_id?: string;
		image_url?: string;
	};

	let {
		images = $bindable<Array<OrderableImageUploadItem>>(),
		disabled = false
	}: {
		images: Array<OrderableImageUploadItem>;
		disabled: boolean;
	} = $props();

	let focusedImage = $state<OrderableImageUploadItem>();

	const handleDrop: ChangeEventHandler<HTMLInputElement> = async (event) => {
		const files = event.currentTarget.files;
		if (!files?.length) return;

		const fileList = Array.from(files);
		const latestImageOrder = images.length;

		const newItems = await Promise.all(
			fileList.map((file, index) => {
				return new Promise<OrderableImageUploadItem>((resolve) => {
					const reader = new FileReader();
					reader.onloadend = (e) => {
						resolve({
							order: latestImageOrder + index,
							image: file,
							image_url: (e.target?.result as string) ?? undefined
						});
					};
					reader.readAsDataURL(file);
				});
			})
		);

		images = images.concat(newItems);
		event.currentTarget.value = '';
	};

	const removeImage = (order: number) => {
		if (focusedImage?.order === order) {
			focusedImage = undefined;
		}

		images = images
			.sort((a, b) => a.order - b.order)
			.filter((img) => img.order !== order)
			.map((img, i) => ({ ...img, order: i }));
	};

	const setPrimary = (order: number) => {
		images = images.map((image) => ({
			...image,
			is_primary: image.order === order ? true : false
		}));
	};

	const handleDnd = (e: CustomEvent<{ items: OrderableImageUploadItem[] }>) => {
		images = e.detail.items;
	};
</script>

<div class="flex flex-col gap-4">
	<div class="bg-muted overflow-hidden rounded-lg">
		<img
			src={focusedImage?.image_url}
			alt={focusedImage?.image?.name ?? focusedImage?.image_id ?? ''}
			class="bg-muted h-[min(600px,60dvh)] w-full object-contain"
		/>
	</div>

	<div
		class="grid grid-cols-2 gap-2 sm:grid-cols-3 md:grid-cols-5"
		use:dragHandleZone={{ items: images }}
		onfinalize={handleDnd}
		onconsider={handleDnd}
	>
		{#each images as image (image.order)}
			<div class="relative h-96 w-full overflow-hidden rounded-lg border border-border">
				<div
					use:dragHandle
					aria-label="Drag handle for {image.image?.name ?? image.image_id}"
					class="bg-muted absolute top-0 left-0 z-10 flex size-8 items-center justify-center rounded-br-lg"
				>
					<HugeiconsIcon icon={DragDropVerticalIcon} class="text-foreground size-5" />
				</div>
				<button
					{disabled}
					type="button"
					onclick={() => removeImage(image.order)}
					class="absolute top-0 right-0 z-10 flex size-8 cursor-pointer items-center justify-center rounded-bl-lg bg-destructive/90 hover:bg-destructive"
				>
					<HugeiconsIcon icon={Delete02Icon} class="text-destructive-foreground size-5" />
				</button>
				<button
					{disabled}
					type="button"
					onclick={() => setPrimary(image.order)}
					class="bg-muted/80 hover:bg-muted absolute right-0 bottom-0 z-10 flex size-8 cursor-pointer items-center justify-center rounded-tl-lg"
				>
					{#if image.is_primary}
						<HugeiconsIcon icon={StarIcon} class="size-5 text-amber-500" />
					{:else}
						<HugeiconsIcon icon={StarOffIcon} class="text-muted-foreground size-5" />
					{/if}
				</button>
				<button
					{disabled}
					type="button"
					onclick={() => (focusedImage = image)}
					class="absolute inset-0 h-full w-full cursor-pointer rounded-lg border-0 bg-transparent p-0 hover:opacity-90"
				>
					<img
						src={image.image_url}
						alt={image.image?.name ?? image.image_id}
						class="bg-muted h-full w-full rounded-lg object-cover"
					/>
				</button>
			</div>
		{/each}
	</div>

	<label
		class="border-input bg-background hover:bg-muted/50 flex h-96 cursor-pointer flex-col items-center justify-center gap-2 rounded-lg border border-dashed px-4 text-center text-sm transition-colors {disabled
			? 'pointer-events-none opacity-50'
			: ''}"
	>
		<input
			{disabled}
			type="file"
			accept="image/*"
			multiple
			class="sr-only"
			onchange={handleDrop}
		/>
		<span class="text-muted-foreground">{m.admin_sectors_images_dropzone()}</span>
	</label>
</div>
