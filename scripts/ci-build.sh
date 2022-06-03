#!/bin/bash
set -e

DOCKERFILE="./Dockerfile"

echo "Build starting"


docker build \
    --rm=false --file "$DOCKERFILE" -t ciceksepeti/eventransit .

echo "Build finished"