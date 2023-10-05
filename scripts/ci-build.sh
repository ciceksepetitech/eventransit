#!/bin/bash
set -e

DOCKERFILE="./Dockerfile"

echo "Build starting"


docker build \
    --build-arg NEW_RELIC_LICENSE_KEY="$NEW_RELIC_LICENSE_KEY" \
    --rm=false --file "$DOCKERFILE" -t ciceksepeti/eventransit .

echo "Build finished"
