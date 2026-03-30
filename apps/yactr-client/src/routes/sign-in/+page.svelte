<script lang="ts">
  import { page } from '$app/state';
  import { authClient } from '$lib/auth-client';
	import { Button } from '$lib/components/ui/button';
	import * as Card from '$lib/components/ui/card';
	import { m } from '$lib/paraglide/messages.js';

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
  <Card.Root>
    <Card.Content>
      <Card.Title>{m.sign_in_title()}</Card.Title>
      <Card.Description>{m.sign_in_description()}</Card.Description>
    </Card.Content>
    <Card.Footer class="flex justify-center">
      <Button
        type="button"
        variant="default"
        onclick={handleSignIn}
      >
        {m.sign_in_button()}
      </Button>
    </Card.Footer>
  </Card.Root>
</main>

