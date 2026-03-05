import { betterAuth } from "better-auth";
import { genericOAuth } from "better-auth/plugins";
import { svelteKitHandler, sveltekitCookies } from "better-auth/svelte-kit";
import { getRequestEvent } from "$app/server";
import { building } from "$app/environment";
import type { Handle } from "@sveltejs/kit";
import {
  AUTH_AUTH0_AUDIENCE,
  AUTH_AUTH0_ID,
  AUTH_AUTH0_ISSUER,
  AUTH_AUTH0_SECRET,
  BETTER_AUTH_URL
} from "$env/static/private";
import { yactrApiEndpointsUsersGetCurrentUser } from "./api";

export interface AuthSession {
  access_token: string;
}

export const auth = betterAuth({
  baseURL: new URL(BETTER_AUTH_URL).origin,
  plugins: [
    genericOAuth({
      config: [
        {
          providerId: "auth0",
          discoveryUrl: `${AUTH_AUTH0_ISSUER}/.well-known/openid-configuration`,
          clientId: AUTH_AUTH0_ID,
          clientSecret: AUTH_AUTH0_SECRET,
          issuer: AUTH_AUTH0_ISSUER,
          scopes: ["openid", "profile", "email", "offline_access"],
          accessType: "offline",
          authorizationUrlParams: {
            audience: AUTH_AUTH0_AUDIENCE
          }
        }
      ]
    }),
    // must be last plugin per Better Auth SvelteKit docs
    sveltekitCookies(getRequestEvent)
  ]
});

export const handle: Handle = async ({ event, resolve }) => {
  (event.locals as any).auth = async () => {
    const session = await auth.api.getSession({
      headers: event.request.headers
    });

    if (!session) return null;

    try {
      const tokenResult = await auth.api.getAccessToken({
        body: {
          providerId: "auth0"
        },
        headers: event.request.headers
      });

      const accessToken = tokenResult?.accessToken;

      if (!accessToken) return null;

      const normalized: AuthSession = {
        access_token: accessToken
      };

      return normalized;
    } catch (error) {
      console.error("Failed to get Auth0 access token from Better Auth", error);
      return null;
    }
  };

  return svelteKitHandler({ event, resolve, auth, building });
};

export const getAPIUserHook: Handle = async ({ event, resolve }) => {
  event.locals.user = async () => {
    if (event.cookies.get("ajs_user")) {
      return JSON.parse(event.cookies.get("ajs_user")!);
    }

    if (!event.cookies.get("ajs_user")) {
      const { data } = await yactrApiEndpointsUsersGetCurrentUser();

      if (data) {
        event.cookies.set("ajs_user", JSON.stringify(data), {
          path: "/",
          httpOnly: true,
          secure: true,
          maxAge: 60
        });

        return data;
      }

      return undefined;
    }
  };

  return resolve(event);
};

