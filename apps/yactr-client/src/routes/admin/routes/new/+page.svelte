<script lang="ts">
	import RouteManagementForm from '$lib/forms/route_management/route_management_form.svelte';
	import type { PageProps } from './$types';
	import { Heading, Hr } from 'flowbite-svelte';
	let { data }: PageProps = $props();
</script>

<Heading tag="h1">New Route</Heading>
<Hr />
<RouteManagementForm sectors={data.sectors ?? []} access_token={data.session?.access_token!} />
