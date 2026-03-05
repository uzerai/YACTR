<script lang="ts">
	import '../../app.css';
	import favicon from '$lib/assets/favicon.svg';
	import { authClient } from '$lib/auth-client';

	let { children, data } = $props();
	const session = authClient.useSession();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<nav class="w-full bg-space-cadet flex items-center justify-between px-4 py-2 text-white">
	<div class="font-semibold">
		YACTR
	</div>

	<div class="flex items-center gap-3">
		{#if $session.data}
			<span class="text-sm opacity-80">
				{$session.data.user?.email ?? 'Signed in'}
			</span>
			<button
				class="rounded bg-white/10 px-3 py-1 text-sm hover:bg-white/20"
				onclick={async () => {
					await authClient.signOut();
				}}
			>
				Sign out
			</button>
		{:else}
			<button
				class="rounded bg-white px-3 py-1 text-sm font-medium text-space-cadet hover:bg-gray-100"
				onclick={async () => {
					await authClient.signIn.social({
						provider: 'auth0'
					});
				}}
			>
				Sign in
			</button>
		{/if}
	</div>
</nav>

{@render children?.()}
