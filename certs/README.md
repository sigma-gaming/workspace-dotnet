# certs

Local HTTPS certificates for development are not committed to the repository.
Generate them before the first run:

```bash
mkcert -install
mkcert -key-file certs/local.key -cert-file certs/local.crt \
  localhost sigma.local '*.sigma.local'
```

The paths are expected by `Properties/launchSettings.json` of every app
(`ASPNETCORE_Kestrel__Certificates__Default__{Path,KeyPath}`) and by
`compose/auth-proxy.traefik.yaml`.
