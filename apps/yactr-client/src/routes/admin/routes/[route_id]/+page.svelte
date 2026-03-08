<script lang="ts">
	import { Heading, Hr } from 'flowbite-svelte';
	import type { PageProps } from './$types';
	import RouteManagementForm from '../route_management_form.svelte';
	let { data }: PageProps = $props();
	let route = $derived(data.route);
	let sectors = $derived(data.sectors ?? []);
</script>

<Heading tag="h1">{data.route.name}</Heading>
<Hr />
<RouteManagementForm {route} {sectors} access_token={data.session?.access_token!} />
