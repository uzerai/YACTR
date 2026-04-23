# Local Dex Development IDP

This project includes a local [Dex](https://dexidp.io) service in `docker-compose.yaml` under the `development-full` profile.

## Seeded credentials

- Username: `dev`
- Email: `dev@yactr.local`
- Password: `password`

## Seeded OAuth client

- Client ID: `yactr-dev-client`
- Client Secret: `yactr-dev-secret`
- Redirect URIs:
  - `http://localhost:8080/swagger/oauth2-redirect.html`
  - `http://localhost:5173/auth/callback`

## API auth env values

When running in compose, API auth points to Dex using:

- `Auth0__Domain=http://dex:5556/dex`
- `Auth0__Audience=yactr-dev-client`
- `Auth0__Issuer=http://dex:5556/dex`

## Discovery endpoint

- In Docker network: `http://dex:5556/dex/.well-known/openid-configuration`
- From host: `http://localhost:5556/dex/.well-known/openid-configuration`

## Persistence

Dex uses SQLite storage at `/data/dex.db` and mounts Docker volume `dex-data`.
This keeps Dex signing keys and persisted state across container restarts.
