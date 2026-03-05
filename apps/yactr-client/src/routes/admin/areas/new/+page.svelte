<script lang="ts">
	import AreaManagementForm from '../area_management_form.svelte';
	import type { YactrDomainModelClimbingArea } from '$lib/api';
	import { Heading, Hr } from 'flowbite-svelte';

	let area = $state<YactrDomainModelClimbingArea>();
</script>

<Heading tag="h1" class="">New area</Heading>
<Hr />
<AreaManagementForm bind:area />
