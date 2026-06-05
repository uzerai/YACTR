#!/usr/bin/env just --justfile

mod server 'apps/yactr-server/justfile'
mod client 'apps/yactr-client/justfile'

[private]
default:
  @just --list --unsorted

# ───────────────────────── stack ─────────────────────────

[group('stack')]
[doc('Spin up the entire stack (db, minio, dex, api, client) in Docker')]
up:
  docker compose --profile dev up -d
  just server::run-migrations
  docker compose --profile full up

[group('stack')]
[doc('Stop and remove the whole stack')]
down:
  docker compose --profile full down

[group('stack')]
[doc('Infra in Docker + api & client hot-reload on host (tmux)')]
dev:
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
[doc('Install deps for both apps')]
install:
  just server::restore
  just client::install

[group('stack')]
[doc('Tail logs for the full stack')]
logs:
  docker compose --profile full logs -f
