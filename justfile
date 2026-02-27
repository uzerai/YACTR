#!/usr/bin/env just --justfile

[private]
default:
  @just --list --unsorted

[group('docker-compose')]
[doc('Starts dev-profile docker compose & dotnet watch of server in tmux session which exits both when detached')]
dev:
  tmux \
    new-session -d -s yactr-dev "trap 'docker compose --profile development down' EXIT; docker compose --profile development up" \; \
    set-option -t yactr-dev -g mouse on \; \
    set-option -t yactr-dev destroy-unattached on \; \
    split-window -t yactr-dev 'dotnet watch --project src/YACTR.Api' \; \
    attach -t yactr-dev

[group('docker-compose')]
[doc('Starts test-profile docker compose for debug with default settings')]
test-env:
  tmux \
    new-session -d -s yactr-test "trap 'docker compose --profile test down' EXIT; docker compose --profile test up" \; \
    set-option -t yactr-test -g mouse on \; \
    set-option -t yactr-test destroy-unattached on \; \
    attach -t yactr-test

[group('test')]
[doc('Run the test-profile docker compose and run a test suite with coverage report generation enabled')]
coverage:
  rm -rf ./tests/YACTR.Api.Tests/TestResults
  docker compose --profile test up -d
  -dotnet test YACTR.sln --settings ./tests.runsettings --verbosity minimal --collect:"XPlat Code Coverage"
  docker compose --profile test down
  reportgenerator \
    -reports:"tests/**/TestResults/**/coverage.cobertura.xml" \
    -targetdir:"coverage-report" \
    -reporttypes:Html && \
  open coverage-report/index.html

[group('database')]
[doc('Runs migrations against the dev-profile docker-compose')]
run-migrations environment='development':
    ASPNETCORE_ENVIRONMENT={{environment}} dotnet ef database update --project src/YACTR.Infrastructure --startup-project src/YACTR.Api

[group('database')]
[doc('Creates a migration in the project')]
add-migration migration_name:
    dotnet ef migrations add {{migration_name}} --project src/YACTR.Infrastructure --startup-project src/YACTR.Api -c DatabaseContext

[group('database')]
[doc('Removes the latest migration and recreates it, optionally with a new name')]
recreate-latest-migration new_name='':
    #!/usr/bin/env bash
    if [ -z "{{new_name}}" ]; then
        migration_name=$(dotnet ef migrations list --project src/YACTR.Infrastructure --startup-project src/YACTR.Api -c DatabaseContext \
            | tail -n 1 | sed -nE 's/^[^_]+_([^ ]+).*/\1/p')
    else
        migration_name="{{new_name}}"
    fi
    echo "Recreating latest migration with name: $migration_name"
    dotnet ef migrations remove --project src/YACTR.Infrastructure --startup-project src/YACTR.Api -c DatabaseContext &> /dev/null && echo "Migration removed successfully"
    dotnet ef migrations add "$migration_name" --project src/YACTR.Infrastructure --startup-project src/YACTR.Api -c DatabaseContext &> /dev/null && echo "Migration created successfully"

[group('database')]
[doc('Rolls back the database to a given migration')]
db-rollback migration_name:
    dotnet ef database update {{migration_name}} --project src/YACTR

[group('database')]
[doc('Truncates all tables in the local database, development or test')]
truncate-local-db environment='development':
    PGPASSWORD=yactr psql -h localhost -U yactr -d {{ if environment == 'development' {"yactr"} else {"yactr_test"} }} \
    -c "DO \$\$ DECLARE r RECORD; BEGIN SET session_replication_role = replica; FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public' AND tablename <> 'migrations' AND tablename <> 'users') LOOP EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' CASCADE'; END LOOP; SET session_replication_role = DEFAULT; RAISE NOTICE 'All tables in public schema have been truncated successfully.'; END \$\$"