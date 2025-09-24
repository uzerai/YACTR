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