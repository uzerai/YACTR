#!/usr/bin/env just --justfile

[private]
default:
  just --list --unsorted

dev:
  tmux \
    new-session -d -s yactr-dev "trap 'docker compose --profile development down' EXIT; docker compose --profile development up" \; \
    set-option -t yactr-dev -g mouse on \; \
    set-option -t yactr-dev destroy-unattached on \; \
    split-window -t yactr-dev 'dotnet watch --project src/YACTR/YACTR.csproj' \; \
    attach -t yactr-dev

test:
  tmux \
    new-session -d -s yactr-test "trap 'docker compose --profile test down' EXIT; docker compose --profile test up" \; \
    set-option -t yactr-test -g mouse on \; \
    set-option -t yactr-test destroy-unattached on \; \
    attach -t yactr-test

run-migrations:
    dotnet ef database update --project src/YACTR