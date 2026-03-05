<script lang="ts">
	import SectorManagementForm from '../sector_management_form.svelte';
	import type { YactrDomainModelClimbingSector } from '$lib/api';
	import type { PageProps } from './$types';
	import { Heading, Hr } from 'flowbite-svelte';

	let { data }: PageProps = $props();
	let sector = $state<YactrDomainModelClimbingSector>();
</script>

<Heading tag="h1">New Sector</Heading>
<Hr />
<SectorManagementForm bind:sector areas={data.areas} />
