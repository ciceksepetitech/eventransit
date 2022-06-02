#!/bin/bash
set -e

# variables
configuration=${1:-Debug}

# test
for path in test/*/; do
    (cd $path; dotnet test -c $configuration --no-restore --no-build)
done