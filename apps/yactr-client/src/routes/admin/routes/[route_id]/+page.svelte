<script lang="ts">
	import { Heading, Hr } from 'flowbite-svelte';
	import type { PageProps } from './$types';
	import { RouteManagementForm } from '$lib/components/forms';
	let { data }: PageProps = $props();
	let route = $derived(data.route);
	let sectors = $derived(data.sectors ?? []);
</script>

<Heading tag="h1">{data.route.name}</Heading>
<Hr />
<!-- <RouteManagementForm  {sectors} access_token={data.session?.access_token!} /> -->
