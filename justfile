#!/usr/bin/env just --justfile

[private]
default:
  @just --list --unsorted

[doc('Starts dev-profile docker compose & dotnet watch of server in tmux session which exits both when detached')]
dev:
  tmux \
    new-session -d -s yactr-dev "trap 'docker compose --profile development down' EXIT; docker compose --profile development up" \; \
    set-option -t yactr-dev -g mouse on \; \
    set-option -t yactr-dev destroy-unattached on \; \
    split-window -t yactr-dev 'dotnet watch --project src/YACTR/YACTR.csproj' \; \
    attach -t yactr-dev

[doc('Starts test-profile docker compose for debug with default settings')]
test:
  tmux \
    new-session -d -s yactr-test "trap 'docker compose --profile test down' EXIT; docker compose --profile test up" \; \
    set-option -t yactr-test -g mouse on \; \
    set-option -t yactr-test destroy-unattached on \; \
    attach -t yactr-test

[doc('Runs migrations against the dev-profile docker-compose')]
run-migrations environment='development':
    ASPNETCORE_ENVIRONMENT={{environment}} dotnet ef database update --project src/YACTR

[doc('Creates a migration in the project')]
add-migration migration_name:
    dotnet ef migrations add {{migration_name}} --project src/YACTR

[doc('Rolls back the database to a given migration')]
db-rollback migration_name:
    dotnet ef database update {{migration_name}} --project src/YACTR