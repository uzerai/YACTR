<script lang="ts">
  import { page } from '$app/state';
  import { authClient } from '$lib/auth-client';
	import { Button } from '$lib/components/ui/button';

  const handleSignIn = async () => {
    const redirect =
      page.url.searchParams.get('callbackUrl') ?? "/";

    await authClient.signIn.oauth2({
      providerId: 'auth0',
      callbackURL: redirect
    });
  };
</script>

<main class="w-full min-h-screen flex items-center justify-center">
  <Button
    type="button"
    class="text-antiflash-white cursor-pointer bg-red-100 hover:bg-red-700 px-4 py-2 rounded"
    onclick={handleSignIn}
  >
    Sign in with Auth0
  </Button>
</main>

