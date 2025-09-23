#!/usr/bin/env just --justfile

[private]
default:
  just --list --unsorted

run-dev:
  tmux \
    new-session -d -s yactr-dev "trap 'docker compose --profile development down' EXIT; docker compose --profile development up" \; \
    set-option -t yactr-dev destroy-unattached on \; \
    split-window -t yactr-dev 'dotnet run --project src/YACTR/YACTR.csproj' \; \
    attach -t yactr-dev