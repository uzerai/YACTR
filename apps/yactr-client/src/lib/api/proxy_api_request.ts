import { getAccessToken } from "$lib/api/authorized_api";
import { YACTR_BASE_API_URL } from "$env/static/private";
import type { RequestEvent } from "@sveltejs/kit";

const FORWARDED_REQUEST_HEADERS = [
  "accept",
  "content-type",
  "if-match",
  "if-none-match",
  "if-modified-since",
  "if-unmodified-since"
] as const;

const FORWARDED_RESPONSE_HEADERS = [
  "content-type",
  "content-disposition",
  "etag",
  "last-modified",
  "location"
] as const;

const METHODS_WITH_BODY = new Set(["POST", "PUT", "PATCH"]);

function buildUpstreamUrl(path: string, search: string): string {
  const base = YACTR_BASE_API_URL.replace(/\/$/, "");
  const normalizedPath = path.replace(/^\//, "");
  return `${base}/${normalizedPath}${search}`;
}

function pickHeaders(source: Headers, allowed: readonly string[]): Headers {
  const headers = new Headers();

  for (const name of allowed) {
    const value = source.get(name);
    if (value) {
      headers.set(name, value);
    }
  }

  return headers;
}

export async function proxyApiRequest(event: RequestEvent): Promise<Response> {
  const path = event.params.path;

  if (!path) {
    return new Response("Missing API path", { status: 400 });
  }

  const upstreamUrl = buildUpstreamUrl(path, event.url.search);
  const headers = pickHeaders(event.request.headers, FORWARDED_REQUEST_HEADERS);

  const accessToken = await getAccessToken(event);
  if (accessToken) {
    headers.set("Authorization", `Bearer ${accessToken}`);
  }

  const method = event.request.method;
  const hasBody = METHODS_WITH_BODY.has(method) && event.request.body !== null;

  const upstreamResponse = await fetch(upstreamUrl, {
    method,
    headers,
    body: hasBody ? event.request.body : undefined,
    ...(hasBody ? { duplex: "half" } : {})
  } as RequestInit);

  const responseHeaders = pickHeaders(upstreamResponse.headers, FORWARDED_RESPONSE_HEADERS);

  return new Response(upstreamResponse.body, {
    status: upstreamResponse.status,
    statusText: upstreamResponse.statusText,
    headers: responseHeaders
  });
}
