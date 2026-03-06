<script lang="ts">
	import AreaManagementForm from '../area_management_form.svelte';
	import { Heading, Hr } from 'flowbite-svelte';

	let { data } = $props();
</script>

<Heading tag="h1" class="">New area</Heading>
<Hr />
<AreaManagementForm data={data.form} />
