<script lang="ts">
	import { type SuperValidated } from 'sveltekit-superforms';
	import { z } from 'zod';
	import RouteManagementForm from '../route_management_form.svelte';
	import type { PageProps } from './$types';
	import { Heading, Hr } from 'flowbite-svelte';
	import { zRouteResponse } from '$lib/api/generated/zod.gen';
	let { data }: PageProps = $props();
</script>

<Heading tag="h1">New Route</Heading>
<Hr />
<RouteManagementForm sectors={data.sectors!} data={data.form as SuperValidated<z.infer<typeof zRouteResponse>>}  />
