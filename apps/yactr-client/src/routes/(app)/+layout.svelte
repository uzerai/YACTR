<script lang="ts">
	import '../../app.css';
	import favicon from '$lib/assets/favicon.svg';
  import { SignIn, SignOut } from "@auth/sveltekit/components";

	let { children, data } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<nav class="w-full bg-space-cadet flex">
  {#if data.session}
    <SignOut signOutPage='sign-out'>
			<span slot="submitButton" class="text-antiflash-white cursor-pointer bg-red-100 hover:bg-red-700">Sign out</span>
		</SignOut>
  {:else}
    <SignIn provider='auth0' signInPage='sign-in'>
      <span slot="submitButton" class="text-antiflash-white cursor-pointer bg-red-100 hover:bg-red-700">Sign in</span>
    </SignIn>
  {/if}
</nav>

{@render children?.()}
