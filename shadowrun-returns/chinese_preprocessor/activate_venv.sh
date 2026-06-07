#!/usr/bin/env bash
# Usage: source ./activate_venv.sh

if [[ "${BASH_SOURCE[0]}" == "$0" ]]; then
  echo "Source this script instead of executing it: source ./activate_venv.sh" >&2
  exit 1
fi

_SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [[ ! -d "$_SCRIPT_DIR/.venv" ]]; then
  echo "No .venv found. Run ./setup_venv.sh first." >&2
  return 1 2>/dev/null || exit 1
fi

source "$_SCRIPT_DIR/.venv/bin/activate"
