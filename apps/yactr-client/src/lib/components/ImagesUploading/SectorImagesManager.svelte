<script lang="ts">
  import { Dropzone, Gallery, P } from "flowbite-svelte";
  import type { ChangeEventHandler } from "svelte/elements";
  import { dragHandle, dragHandleZone } from "svelte-dnd-action";
	import { ObjectsColumnSolid, StarOutline, StarSolid, TrashBinSolid } from "flowbite-svelte-icons";

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
  };

  // Removes the image and maintains the order of the images after it.
  const removeImage = (order: number) => {
    if (focusedImage?.order === order) {
      focusedImage = undefined;
    }
    
    images = images.sort((a, b) => a.order - b.order)
      .filter(img => img.order !== order).map((img, i) => ({ ...img, order: i }));
  };

  const setPrimary = (order: number) => {
    images = images.map(image => ({ ...image, is_primary: image.order === order ? true : false }))
  }

  const handleDnd = (e: CustomEvent<{ items: OrderableImageUploadItem[] }>) => {
    images = e.detail.items;
  };
</script>

<div>
  <Gallery class="gap-4">
    <img src={focusedImage?.image_url} alt={focusedImage?.image?.name ?? focusedImage?.image_id} class="h-[600px] w-full rounded-lg bg-gray-200 dark:bg-neutral-400 object-contain" />

    <div class="grid grid-cols-5 gap-2" use:dragHandleZone={{ items: images  }} onfinalize={handleDnd} onconsider={handleDnd}>
      {#each images as image (image.order)}
        <div class="rounded-lg border-0 bg-transparent p-0 relative w-full h-96 overflow-hidden">
          <div use:dragHandle aria-label="drag-handle for {image.image?.name ?? image?.image_id}" class="size-8 z-10 bg-neutral-600 absolute top-0 left-0 flex rounded-br-lg items-center justify-center">
            <ObjectsColumnSolid class="shrink-0 h-6 w-6 text-neutral-800" />
          </div>
          <button {disabled} type="button" onclick={() => removeImage(image.order)} class="z-10 cursor-pointer rounded-bl-lg border-0 bg-red-500 p-0 hover:opacity-80 absolute top-0 right-0 size-8 flex items-center justify-center">
            <TrashBinSolid class="shrink-0 h-6 w-6 text-neutral-200" />
          </button>
          <button {disabled}type="button" onclick={() => setPrimary(image.order)} class="z-10 cursor-pointer rounded-tl-lg border-0 bg-neutral-400/50 p-0 hover:opacity-80 absolute bottom-0 right-0 size-8 flex items-center justify-center">
            {#if image.is_primary}
              <StarSolid class="shrink-0 h-6 w-6 text-yellow-500" />
            {:else}
              <StarOutline class="shrink-0 h-6 w-6 text-neutral-200" />
            {/if}
          </button>
          <button {disabled} type="button" onclick={() => focusedImage = image} class="cursor-pointer rounded-lg border-0 bg-transparent p-0 hover:opacity-80 absolute w-full h-full">
            <img src={image.image_url} alt={image.image?.name ?? image.image_id} class="w-full h-full rounded-lg bg-gray-100 object-cover" />
          </button>
        </div>
        
      {/each}
    </div>
    <Dropzone {disabled}accept="image/*" multiple onChange={handleDrop} class="h-96">
      <P>Drag and drop files here or click to upload</P>
    </Dropzone>
  </Gallery>
</div>