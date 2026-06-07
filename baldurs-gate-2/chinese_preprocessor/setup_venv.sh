#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

if [[ ! -d .venv ]]; then
  python3 -m venv .venv
fi

.venv/bin/pip install --upgrade pip
.venv/bin/pip install -r requirements.txt

echo "Virtual environment ready. Activate with: source ./activate_venv.sh"
