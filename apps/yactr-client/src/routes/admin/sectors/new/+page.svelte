<script lang="ts">
	import SectorManagementForm from '../sector_management_form.svelte';
	import type { PageProps } from './$types';
	import { Heading, Hr } from 'flowbite-svelte';

	let { data }: PageProps = $props();
</script>

<Heading tag="h1">New Sector</Heading>
<Hr />
<SectorManagementForm data={data.form} areas={data.areas} />
