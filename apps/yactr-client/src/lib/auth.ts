import { SvelteKitAuth } from "@auth/sveltekit"
import Auth0 from "@auth/sveltekit/providers/auth0"
import { dev } from "$app/environment";
import { AUTH_AUTH0_AUDIENCE, AUTH_AUTH0_ID, AUTH_AUTH0_ISSUER, AUTH_AUTH0_SECRET } from "$env/static/private";
import type { Handle } from "@sveltejs/kit";
import { yactrEndpointsUsersGetCurrentUser } from "./api";

// Override Session type to contain the access_token.
declare module "@auth/sveltekit" {
  interface Session {
    access_token?: string;
  }
}

// Override JWT to contain access_token
declare module "@auth/core/jwt" {
  interface JWT {
    access_token?: string;
    refresh_token?: string;
    expires_at?: number;
  }
}

/// This file works as the main import point for the @auth/sveltekit library.
/// Entirely followed from their step-by-step guide at:
///   https://authjs.dev/getting-started/authentication/oauth
export const { handle, signIn, signOut } = SvelteKitAuth({
  providers: [Auth0({
    authorization: {
      params: {
        audience: AUTH_AUTH0_AUDIENCE,
        scope: "openid profile email offline_access"
      }
    },
  })],
  debug: dev ? true : false,
  callbacks: {
    async session({ session, token }) {
      // Get the access token into the session
      if (token.access_token) {
        session.access_token = token.access_token as string;
      }

      return session;
    },
    async jwt({ token, account }) {
      // Get the access token into the JWT
      if (account) {
        token.access_token = account.access_token;
        token.refresh_token = account.refresh_token;
        token.expires_at = account.expires_at;
      } else if (!!token.expires_at && Date.now() < token.expires_at * 1000) {
        // User is signed in already (with valid token in jwt)
        return token;
      } else {
        if (!token.refresh_token) throw new TypeError("Missing token");


        try {
          const tokenUrl = `${AUTH_AUTH0_ISSUER}/oauth/token`;
          const tokenResponse = await fetch(tokenUrl, {
            method: "POST",
            body: new URLSearchParams({
              client_id: AUTH_AUTH0_ID,
              client_secret: AUTH_AUTH0_SECRET,
              grant_type: "refresh_token",
              refresh_token: token.refresh_token as string,
            }),
          });

          const tokensOrError = await tokenResponse.json();

          if (!tokenResponse.ok) throw tokensOrError;

          const newTokens = tokensOrError as {
            access_token: string
            expires_in: number
            refresh_token?: string
          }

          return {
            ...token,
            access_token: newTokens.access_token,
            expires_at: Math.floor(Date.now() / 1000 + newTokens.expires_in),
            // Some providers only issue refresh tokens once, so preserve if we did not get a new one
            refresh_token: newTokens.refresh_token
              ? newTokens.refresh_token
              : token.refresh_token,
          }
        } catch (error) {
          console.error(error);

          return token;
        }
      }

      return token;
    }
  }
});

export const getAPIUserHook: Handle = async ({ event, resolve }) => {
  event.locals.user = async () => {
    if (event.cookies.get("ajs_user")) {
      return JSON.parse(event.cookies.get("ajs_user")!);
    }

    if (!event.cookies.get("ajs_user")) {
      const { data } = await yactrEndpointsUsersGetCurrentUser();

      if (data) {
        // Using cookie as internal cache for the user object, only lasts 1 minute.
        event.cookies.set("ajs_user", JSON.stringify(data), {
          path: "/",
          httpOnly: true,
          secure: true,
          maxAge: 60 // 1 minute
        });

        return data;
      }

      return undefined;
    }
  }

  return resolve(event);
};