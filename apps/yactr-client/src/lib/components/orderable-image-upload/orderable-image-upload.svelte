<script lang="ts">
	import type { ChangeEventHandler } from 'svelte/elements';
	import * as Item from "$lib/components/ui/item";
	import { dragHandle, dragHandleZone } from 'svelte-dnd-action';
	import { m } from '$lib/paraglide/messages.js';
	import { HugeiconsIcon as Icon } from '@hugeicons/svelte';
	import {
		Delete02Icon,
		DragDropHorizontalIcon,
		StarIcon,
		StarOffIcon
	} from '@hugeicons/core-free-icons';
	import { Button } from '$lib/components/ui/button';

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

	// Keep updating `images` during `consider` so the drag animation has the correct state.
	// Persist/normalize the `order` fields only on `finalize`.
	const handleDndConsider = (e: CustomEvent<{ items: OrderableImageUploadItem[] }>) => {
		images = e.detail.items;
	};

	const handleDndFinalize = (e: CustomEvent<{ items: OrderableImageUploadItem[] }>) => {
		// Normalize order so it's contiguous and matches the final visual order.
		// Mutate in-place to avoid breaking references (e.g. if `focusedImage` points to one of these items).
		e.detail.items.forEach((item, index) => {
			item.order = index;
		});

		images = e.detail.items;
	};
</script>

<div class="flex flex-col gap-4">
		<img
			src={focusedImage?.image_url}
			alt={focusedImage?.image?.name ?? focusedImage?.image_id ?? ''}
			class="bg-muted h-[min(600px,60dvh)] object-contain"
		/>
	<div
		class="grid grid-cols-2 gap-2 sm:grid-cols-3 md:grid-cols-5"
		use:dragHandleZone={{ items: images }}
		onfinalize={handleDndFinalize}
		onconsider={handleDndConsider}
	>
		{#each images as image (image.order)}
			<Item.Root variant="outline">
				<Item.Header class="aspect-square">
					<button
						{disabled}
						type="button"
						onclick={() => (focusedImage = image)}
						class="h-full w-full cursor-pointer border-0 bg-transparent p-0 hover:opacity-90"
					>
						<img
							src={image.image_url}
							alt={image.image?.name ?? image.image_id}
							class="bg-muted h-full w-full object-contain"
						/>
					</button>
				</Item.Header>

				<Item.Content>
					<Item.Title class="text-xs wrap-anywhere">{image.image?.name ?? image.image_id}</Item.Title>
					<Item.Actions class="flex justify-between">
						<Button variant="outline" onclick={() => setPrimary(image.order)} {disabled}>
							{#if image.is_primary}
								<Icon icon={StarIcon} class="size-5 text-amber-500" />
							{:else}
								<Icon icon={StarOffIcon} class="text-muted-foreground size-5" />
							{/if}
						</Button>
						<Button variant="destructive" onclick={() => removeImage(image.order)} {disabled}>
							<Icon icon={Delete02Icon} class="text-destructive-foreground size-5" />
						</Button>
					</Item.Actions>
				</Item.Content>
			
				<Item.Footer>
					<div
						use:dragHandle
						aria-label="Drag handle for {image.image?.name ?? image.image_id}"
						class="bg-muted w-full flex justify-center"
					>
						<Icon icon={DragDropHorizontalIcon} class="text-foreground size-5" />
					</div>
				</Item.Footer>
			</Item.Root>
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
