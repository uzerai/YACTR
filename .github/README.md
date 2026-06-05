```
                              __   __ _______ _______ _______  ______
                                \_/   |_____| |          |    |_____/
                                 |    |     | |_____     |    |    \_
```
<p align="center"><strong>Y</strong>et <strong>A</strong>nother <strong>C</strong>limbing <strong>TR</strong>acker</p>
<div align="center">

![.NET 10.0](https://img.shields.io/badge/Version-%2010.0-informational?style=flat&logo=dotnet)
&nbsp;
![Built With Docker](https://img.shields.io/badge/Built_With-Docker-informational?style=flat&logo=docker)
&nbsp;
[![Test Coverage](https://github.com/uzerai/YACTR/actions/workflows/test-coverage.yaml/badge.svg?branch=main)](https://github.com/uzerai/YACTR/actions/workflows/test-coverage.yaml)
&nbsp;
[![Image Build](https://github.com/uzerai/YACTR/actions/workflows/build-image.yaml/badge.svg?branch=main)](https://github.com/uzerai/YACTR/actions/workflows/build-image.yaml)

</div>

<p align="center">
  <a href="#why">Objective</a> •
  <a href="#features">Features</a> •
  <a href="#development">Development</a> •
  <a href="#contributing">Contributing</a>
</p>

> This is currently WIP and full documentation for usage and 
installation are yet to be complete or maintained.

----

 **YACTR** is a climbing tracker whose intention is to simplify the collection of crags, ascents, bolting/maintenance organizations and climbing community specific information.

Monorepo layout:

- `apps/yactr-server` — .NET API
- `apps/yactr-client` — SvelteKit frontend

## Objective
The final objective of the software in this repository is to be able to serve an single or multi-instance, cloneable, API container which can be easily set up and managed by anyone, for free, to track climbing information with a focus on community management through a granular permission system.

This in stark contrast to a subsection of climbing tracking apps and sites who only serve their subscription based users the complete set of information about the sport.

The need for this arose when I realized there's no plug-and-play, user-friendly solution for a problem I _believe_ smaller climbing groups face when setting up their own routes.
Either going public on the larger route aggregation websited (theCrag / 27Crags / 8a.nu / etc.), and forfeiting administrative rights over crags they themselves maintain; or building a solution through no-code or low-code platforms which usually ends up unmaintained and feature-incomplete (which this project likely will mimic one part of).


## Features
> WIP (documentation, not features)

## Development

### Requirements
- Dotnet 10.0
    - Was it the right choice for this project? Likely not,
    but it's what I was building and RBAC system in anyways.
- Docker
    - Most development relies on the dockerization of subservices, and as such it's best to have it installed. The output of our releases is also a docker image (moving to open-image standard at some point i guess).
- [github.com/casey/just](https://github.com/casey/just) 
    - for running of justfile commands (see the justfile), makes development a lot easier instead of having to fuck around with `dotnet` sdk commands I always forget _some_ option to.

### One-time host setup

Add Dex to your hosts file so OAuth redirects work (browser must resolve `dex` to localhost):

```
127.0.0.1   dex
```

Required for both `just dev` and `just up` (issuer is `http://dex:5556/dex`).

### Getting started

```bash
just install   # creates appsettings.Development.json + .env, installs deps
```

### `just dev` — daily development (host hot-reload)

Starts infra in Docker (Postgres, Minio, Dex), runs migrations, then opens a tmux session with the API (`dotnet watch` on `:5016`) and client (`bun run dev` on `:5173`) on the host.

```bash
just dev
```

Uses `apps/yactr-client/.env` with `YACTR_BASE_API_URL=http://localhost:5016`.

### `just up` — end-to-end Docker stack (staging substitute)

Brings up the full stack in Docker: infra, API, and client. Regenerates the typed REST client from the running API before starting the client container.

```bash
just up
```

Requires `just install` first (client container reads secrets from `apps/yactr-client/.env`; compose overrides API URL and issuer for Docker networking). Browser API calls use same-origin `/api/*`; SvelteKit proxies them to `YACTR_BASE_API_URL`.

Sign in with `dev@yactr.local` / `password` (see `apps/yactr-server/container/dex/README.md`).

### Other commands

```bash
just down                  # stop all containers
just update-client-sdk     # start server profile + regenerate OpenAPI client
just api-generate          # regenerate client against :8080 (default)
just api-generate http://localhost:5016   # regenerate against host API (dev)
```

### Regenerating the API client

When the server schema changes:

```bash
just api-generate http://localhost:5016   # during just dev
just update-client-sdk                  # against Docker API on :8080
```

## Contributing
