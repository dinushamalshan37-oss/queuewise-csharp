#!/usr/bin/env bash
set -euo pipefail
base="${1:-http://127.0.0.1:5090}"
curl -fsS "$base/api/services" >/dev/null
response="$(curl -fsS -X POST "$base/api/tickets" -H 'Content-Type: application/json' -d '{"name":"GitHub Demo","service":"IT Support"}')"
printf 'Created ticket: %s\n' "$response"
curl -fsS "$base/api/tickets" >/dev/null
printf 'API smoke test passed.\n'
