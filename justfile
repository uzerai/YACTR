#!/usr/bin/env just --justfile

mod server 'apps/yactr-server/justfile'
mod client 'apps/yactr-client/justfile'

[private]
default:
  @just --list --unsorted

[private]
preflight-dex:
  #!/usr/bin/env bash
  if ! grep -Eq '^\s*127\.0\.0\.1\s+(\S+\s+)*dex(\s|$)' /etc/hosts; then
    echo "error: 'dex' must resolve to 127.0.0.1 for OAuth (browser redirect)."
    echo "       Add this line to /etc/hosts:"
    echo "           127.0.0.1   dex"
    exit 1
  fi

# ───────────────────────── stack ─────────────────────────

[group('stack')]
[doc('Spin up the entire stack (db, minio, dex, api, client) in Docker')]
up: preflight-dex update-client-sdk
  docker compose --profile full up -d client
  docker compose --profile full logs -f

[group('stack')]
[doc('Stop and remove the whole stack')]
down:
  docker compose --profile dev --profile full --profile server --profile test down

[group('stack')]
[doc('Infra in Docker + api & client hot-reload on host (tmux)')]
dev: preflight-dex
  docker compose --profile dev up -d
  just server::run-migrations
  tmux \
    new-session -d -s yactr "just server::watch" \; \
    set-option -t yactr -g mouse on \; \
    set-option -t yactr destroy-unattached on \; \
    split-window -t yactr "just client::dev" \; \
    attach -t yactr

[group('stack')]
[doc('Start infra + API in Docker (server profile, no client)')]
server-up:
  docker compose --profile server up -d
  just server::run-migrations

[group('stack')]
[doc('Regenerate client REST SDK from a running API (default: compose server on :8080)')]
api-generate url='http://localhost:8080':
  #!/usr/bin/env bash
  set -euo pipefail
  swagger="{{url}}/swagger/v1/swagger.json"
  echo "Waiting for ${swagger}..."
  for _ in $(seq 1 30); do
    curl -sf "$swagger" > /dev/null && break
    sleep 2
  done
  curl -sf "$swagger" > /dev/null || { echo "API not ready at {{url}}"; exit 1; }
  YACTR_BASE_API_URL="{{url}}" just client::api-generate

[group('stack')]
[doc('Start server in Docker and regenerate the client REST SDK from swagger')]
update-client-sdk: server-up api-generate

[group('stack')]
[doc('Create local config from examples, then install deps for both apps')]
install:
  #!/usr/bin/env bash
  set -euo pipefail
  server_dev="apps/yactr-server/src/YACTR.Api/appsettings.Development.json"
  server_example="apps/yactr-server/src/YACTR.Api/appsettings.Example.json"
  client_env="apps/yactr-client/.env"

  if [ ! -f "$server_dev" ]; then
    cp "$server_example" "$server_dev"
    echo "Created $server_dev"
  else
    echo "Skipping $server_dev (already exists)"
  fi

  if [ ! -f "$client_env" ]; then
    secret="$(openssl rand -base64 32)"
    client_port=5173
    api_port=5016
    {
      printf '%s\n' "BETTER_AUTH_SECRET=${secret}"
      printf '%s\n' "BETTER_AUTH_URL=http://localhost:${client_port}"
      printf '%s\n' ""
      printf '%s\n' "AUTH_AUTH0_ID=yactr-dev-client"
      printf '%s\n' "AUTH_AUTH0_SECRET=yactr-dev-secret"
      printf '%s\n' "AUTH_AUTH0_ISSUER=http://dex:5556/dex"
      printf '%s\n' "AUTH_AUTH0_AUDIENCE=yactr-dev-client"
      printf '%s\n' ""
      printf '%s\n' "YACTR_BASE_API_URL=http://localhost:${api_port}"
    } > "$client_env"
    echo "Created $client_env"
  else
    echo "Skipping $client_env (already exists)"
  fi

  just server::restore
  just client::install

[group('stack')]
[doc('Tail logs for the full stack')]
logs:
  docker compose --profile full logs -f
