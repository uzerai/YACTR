import { proxyApiRequest } from "$lib/api/proxy_api_request";
import type { RequestHandler } from "./$types";

const handler: RequestHandler = async (event) => proxyApiRequest(event);

export const GET = handler;
export const POST = handler;
export const PUT = handler;
export const PATCH = handler;
export const DELETE = handler;
export const OPTIONS = handler;
